using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Sample.ShardedClient;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureDiscordShardedHost((context, config) =>
    {
        config.SocketConfig = new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Verbose,
            AlwaysDownloadUsers = true,
            MessageCacheSize = 200,
            TotalShards = 4
        };

        config.Token = context.Configuration["Token"];
    })
    //Omit this if you don't use the command service
    .UseCommandService((context, config) =>
    {
        config.DefaultRunMode = RunMode.Async;
        config.CaseSensitiveCommands = false;
    })
    .ConfigureServices((context, services) =>
    {
        //Add any other services here
        services.AddHostedService<CommandHandler>();
        services.AddHostedService<BotStatusService>();
    }).Build();

await host.RunAsync();