#region License
/*
   Copyright 2018 Hawxy

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
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.Hosting
{
    internal class DiscordHostedService : IHostedService
    {
        private readonly ILogger<DiscordHostedService> _logger;
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _config;

        public DiscordHostedService(ILogger<DiscordHostedService> logger, IConfiguration config, LogAdapter<DiscordSocketClient> adapter, DiscordSocketClient client)
        {
            _logger = logger;
            _config = config;
            _client = client;
            _client.Log += adapter.Log;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Discord.Net hosted service is starting");
            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            var task = _client.StartAsync();
            await Task.WhenAny(task, Task.Delay(-1, cancellationToken));
            if(cancellationToken.IsCancellationRequested) _logger.LogWarning("Startup has been aborted, exiting...");

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Discord.Net hosted service is stopping");
            var task = _client.StopAsync();
            await Task.WhenAny(task, Task.Delay(-1, cancellationToken));
            if (cancellationToken.IsCancellationRequested) _logger.LogCritical("Discord.NET client could not be stopped within the given timeout and may have permanently deadlocked");
        }
    }
}
