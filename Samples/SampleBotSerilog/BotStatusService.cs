using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Sample.Serilog
{
    public class BotStatusService : DiscordClientService
    {
        public BotStatusService(ILogger<DiscordClientService> logger, DiscordSocketClient client) : base(logger, client)
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Entered status service");
            await Client.WaitForReadyAsync(stoppingToken);
            Logger.LogInformation("Client is ready");

            await Client.SetActivityAsync(new Game("Set my status!"));
        }
    }
}
