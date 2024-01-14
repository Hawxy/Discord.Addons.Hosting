using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Sample.ShardedClient;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDiscordShardedHost((config, _) =>
{
    config.SocketConfig = new DiscordSocketConfig
    {
        LogLevel = LogSeverity.Verbose,
        AlwaysDownloadUsers = true,
        MessageCacheSize = 200,
    };

    config.Token = builder.Configuration["Token"]!;
});

builder.Services.AddCommandService((config, _) =>
{
    config.DefaultRunMode = RunMode.Async;
    config.CaseSensitiveCommands = false;
});

builder.Services.AddInteractionService((config, _) =>
{
    config.LogLevel = LogSeverity.Info;
    config.UseCompiledLambda = true;
});


builder.Services.AddHostedService<CommandHandler>();
builder.Services.AddHostedService<InteractionHandler>();
builder.Services.AddHostedService<BotStatusService>();

var host = builder.Build();

await host.RunAsync();

await host.RunAsync();