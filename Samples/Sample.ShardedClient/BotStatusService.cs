using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;

namespace Sample.ShardedClient;

public class BotStatusService : DiscordShardedClientService
{
    public BotStatusService(DiscordShardedClient client, ILogger<BotStatusService> logger) : base(client, logger)
    {
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Client.WaitForReadyAsync(stoppingToken);
        Logger.LogInformation("Client is ready!");

        await Client.SetActivityAsync(new Game("Set my status!"));
    }
}