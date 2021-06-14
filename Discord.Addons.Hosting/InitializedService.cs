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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Discord.Addons.Hosting
{
    /// <summary>
    /// Base class for implementing an <see cref="IHostedService"/> with one-time setup requirements.
    /// </summary>
#if NET
    [Obsolete("Replace with DiscordClientService. See the Discord.Addons.Hosting release notes for more information.", DiagnosticId
 = "DAH001", UrlFormat = "https://github.com/Hawxy/Discord.Addons.Hosting/releases/")]
#else
    [Obsolete("Replace with DiscordClientService. See the Discord.Addons.Hosting release notes for more information.")]
#endif
    public abstract class InitializedService : IHostedService
    {
        /// <summary>
        /// This method is called when the <see cref="IHostedService"/> starts for the first time.
        /// </summary>
        /// <param name="cancellationToken">Triggered when <see cref="IHostedService"/> is stopped during startup.</param>
        public abstract Task InitializeAsync(CancellationToken cancellationToken);

        /// <inheritdoc cref="IHostedService.StartAsync"/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitializeAsync(cancellationToken);
        }

        /// <inheritdoc cref="IHostedService.StopAsync"/>
        public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
