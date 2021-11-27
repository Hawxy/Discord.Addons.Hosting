using Discord;
using Discord.Commands;

namespace Sample.ShardedClient;

public class PublicModule : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<PublicModule> _logger;
    //You can inject the host. This is useful if you want to shutdown the host via a command, but be careful with it.
    private readonly IHost _host;

    public PublicModule(IHost host, ILogger<PublicModule> logger)
    {
        _host = host;
        _logger = logger;
    }

    [Command("ping")]
    [Alias("pong", "hello")]
    public async Task PingAsync()
    {
        _logger.LogInformation("User {user} used the ping command!", Context.User.Username);
        await ReplyAsync("pong!");
    }

    [Command("shutdown")]
    public Task Stop()
    {
        _ = _host.StopAsync();
        return Task.CompletedTask;
    }

    [Command("log")]
    public Task TestLogs()
    {
        _logger.LogTrace("This is a trace log");
        _logger.LogDebug("This is a debug log");
        _logger.LogInformation("This is an information log");
        _logger.LogWarning("This is a warning log");
        _logger.LogError(new InvalidOperationException("Invalid Operation"), "This is a error log with exception");
        _logger.LogCritical(new InvalidOperationException("Invalid Operation"), "This is a critical load with exception");

        _logger.Log(GetLogLevel(LogSeverity.Error), "Error logged from a Discord LogSeverity.Error");
        _logger.Log(GetLogLevel(LogSeverity.Info), "Information logged from Discord LogSeverity.Info ");

        return Task.CompletedTask;
    }

    private static LogLevel GetLogLevel(LogSeverity severity)
        => (LogLevel)Math.Abs((int)severity - 5);
}