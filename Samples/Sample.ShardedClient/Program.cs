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
    .UseCommandService((context, config) =>
    {
        config.DefaultRunMode = RunMode.Async;
        config.CaseSensitiveCommands = false;
    })
    .UseInteractionService((context, config) =>
    {
        config.LogLevel = LogSeverity.Info;
        config.UseCompiledLambda = true;
    })
    .ConfigureServices((context, services) =>
    {
        //Add any other services here
        services.AddHostedService<CommandHandler>();
        services.AddHostedService<InteractionHandler>();
        services.AddHostedService<BotStatusService>();
    }).Build();

await host.RunAsync();