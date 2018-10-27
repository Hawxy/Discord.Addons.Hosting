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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.Hosting
{
    internal class DiscordHostedService<T> : IHostedService, IDisposable where T: BaseSocketClient, new()
    {
        private readonly ILogger<DiscordHostedService<T>> _logger;
        private readonly T _client;
        private readonly IConfiguration _config;

        public DiscordHostedService(ILogger<DiscordHostedService<T>> logger, T client, IConfiguration config, IServiceProvider services)
        {
            _logger = logger;
            _client = client;
            _config = config;

            var adapter = services.GetRequiredService<LogAdapter>();
            //workaround for correct logging category
            adapter.UseLogger(logger);

            //In cases where the constructor is called multiple times
            client.Log -= adapter.Log;
            client.Log += adapter.Log;
            var cs = services.GetService<CommandService>();
            if (cs != null)
            {
                cs.Log -= adapter.Log;
                cs.Log += adapter.Log;
            }
                
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Discord.Net hosted service is starting");
            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Discord.Net hosted service is stopping");
            await _client.StopAsync();
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing Discord.Net hosted service");
            _client.Dispose();
        }
    }
}
