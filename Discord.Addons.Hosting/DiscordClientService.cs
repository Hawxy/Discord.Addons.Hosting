#region License
/*
   Copyright 2021 Hawxy

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
#endregion
using System;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.Hosting
{
    /// <summary>
    /// Base class for implementing an <see cref="IHostedService"/> with one-time setup requirements.
    /// </summary>
#if NET
    [Obsolete("Replace with DiscordClientService. See the Discord.Addons.Hosting release notes for more information.", DiagnosticId = "DAH001", UrlFormat = "https://github.com/Hawxy/Discord.Addons.Hosting/releases/")]
#else
    [Obsolete("Replace with DiscordClientService. See the Discord.Addons.Hosting release notes for more information.")]
#endif
    public abstract class InitializedService : IHostedService
    {
        private bool _initialized;

        /// <summary>
        /// This method is called when the <see cref="IHostedService"/> starts for the first time.
        /// </summary>
        /// <param name="cancellationToken">Triggered when <see cref="IHostedService"/> is stopped during startup.</param>
        public abstract Task InitializeAsync(CancellationToken cancellationToken);

        /// <inheritdoc cref="IHostedService.StartAsync"/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_initialized) return;
            await InitializeAsync(cancellationToken);
            if (!cancellationToken.IsCancellationRequested) _initialized = true;
        }

        /// <inheritdoc cref="IHostedService.StopAsync"/>
        public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask; 
    }


    /// <summary>
    /// Base class for implementing an <see cref="DiscordClientService"/> with startup execution requirements. This class implements <see cref="BackgroundService"/>
    /// </summary>
    public abstract class DiscordClientService : BackgroundService
    {
        /// <summary>
        /// The running Discord.NET Client
        /// </summary>
        protected DiscordSocketClient Client { get; }
        /// <summary>
        /// This service's logger
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Creates a new <see cref="DiscordClientService" />
        /// </summary>
        /// <param name="logger">The logger for this service</param>
        /// <param name="client">The discord client</param>
        protected DiscordClientService(ILogger<DiscordClientService> logger, DiscordSocketClient client)
        {
            Client = client;
            Logger = logger;
        }

        /// <summary>
        /// This method is called when the <see cref="IHostedService"/> starts. If the implementation is long-running, it should return a task that represents
        /// the lifetime of the operation(s) being performed.
        /// For more information, see <see href=" https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services#backgroundservice-base-class"/>
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        protected abstract override Task ExecuteAsync(CancellationToken stoppingToken);

    }
}
