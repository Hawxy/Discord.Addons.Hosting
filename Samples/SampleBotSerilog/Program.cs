using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    // requires Serilog.Extensions.Hosting
    builder.Services.AddSerilog();
        
    builder.Services.AddDiscordHost((config, _) =>
    {
        config.SocketConfig = new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Verbose,
            AlwaysDownloadUsers = true,
            MessageCacheSize = 200,
            GatewayIntents = GatewayIntents.All
        };

        config.Token = builder.Configuration["Token"]!;
        
        //Use this to configure a custom format for Client/CommandService logging if needed. The default is below and should be suitable for Serilog usage
        config.LogFormat = (message, exception) => $"{message.Source}: {message.Message}";
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

    var host = builder.Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}