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

using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.Hosting
{

    /// <summary>
    /// Base class for implementing an <see cref="DiscordClientService"/> with startup execution requirements. This class implements <see cref="BackgroundService"/>
    /// </summary>
    public abstract class DiscordServiceBase<T> : BackgroundService where T : BaseSocketClient
    {
        /// <summary>
        /// The running Discord.NET Client
        /// </summary>
        protected T Client { get; }

        /// <summary>
        /// This service's logger
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Creates a new <see cref="DiscordClientService" />
        /// </summary>
        /// <param name="logger">The logger for this service</param>
        /// <param name="client">The discord client</param>
        protected DiscordServiceBase(T client, ILogger logger)
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

    /// <summary>
    /// Base class for implementing an <see cref="DiscordClientService"/> for a <see cref="DiscordShardedClient"/> with startup execution requirements.
    /// This class implements <see cref="BackgroundService"/> and should be registered in your service collection with `AddHostedService`
    /// </summary>
    public abstract class DiscordClientService : DiscordServiceBase<DiscordSocketClient>
    {
        /// <summary>
        /// Creates a new <see cref="DiscordClientService" />
        /// </summary>
        /// <param name="logger">The logger for this service</param>
        /// <param name="client">The discord client</param>
        protected DiscordClientService(DiscordSocketClient client, ILogger<DiscordClientService> logger) : base(client, logger)
        {
        }

    }

    /// <summary>
    /// Base class for implementing an <see cref="DiscordShardedClientService"/> for a <see cref="DiscordShardedClient"/> with startup execution requirements.
    /// This class implements <see cref="BackgroundService"/> and should be registered in your service collection with `AddHostedService`
    /// </summary>
    public abstract class DiscordShardedClientService : DiscordServiceBase<DiscordShardedClient>
    {
        /// <summary>
        /// Creates a new <see cref="DiscordClientService" />
        /// </summary>
        /// <param name="logger">The logger for this service</param>
        /// <param name="client">The discord client</param>
        protected DiscordShardedClientService(DiscordShardedClient client, ILogger<DiscordShardedClientService> logger) : base(client, logger)
        {
        }
    }
}