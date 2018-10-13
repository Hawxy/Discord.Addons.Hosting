using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace SampleBotSerilog
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        //will be injected
        public ILogger<PublicModule> _logger { get; set; }

        [Command("ping")]
        [Alias("pong", "hello")]
        public async Task PingAsync()
        {
            _logger.LogInformation($"User {Context.User.Username} used the ping command!");
            await ReplyAsync("pong!");
        }
    }
}
