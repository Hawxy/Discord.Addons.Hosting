#region License
/*
   Copyright 2019-2022 Hawxy

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

using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Discord.Addons.Hosting.Services
{
    internal class DiscordHostedService<T> : IHostedService where T: BaseSocketClient
    {
        private readonly ILogger<DiscordHostedService<T>> _logger;
        private readonly T _client;
        private readonly DiscordHostConfiguration _config;

        public DiscordHostedService(ILogger<DiscordHostedService<T>> logger, IOptions<DiscordHostConfiguration> options, LogAdapter<T> adapter, T client)
        {
            _logger = logger;
            _config = options.Value;
            _client = client;
            _client.Log += adapter.Log;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Discord.NET hosted service is starting");
            
            try
            {
                await _client.LoginAsync(TokenType.Bot, _config.Token).WaitAsync(cancellationToken);
                await _client.StartAsync().WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Startup has been aborted, exiting...");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Discord.NET hosted service is stopping");
            try
            {
                await _client.StopAsync().WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogCritical("Discord.NET client could not be stopped within the given timeout and may have permanently deadlocked");
            }
        }
    }
}
