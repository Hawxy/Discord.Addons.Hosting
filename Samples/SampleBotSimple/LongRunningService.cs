using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;

namespace Sample.Simple;

public class LongRunningService : DiscordClientService
{
    public LongRunningService(DiscordSocketClient client, ILogger<DiscordClientService> logger) : base(client, logger)
    {
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for the client to be ready
        await Client.WaitForReadyAsync(stoppingToken);
        //Start a pumping background service that lasts for the length of host's existence

        while (!stoppingToken.IsCancellationRequested)
        {
            Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}