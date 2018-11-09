using System;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting.Reliability;
using Discord.Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleBotSerilog
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        //Will be injected
        public ILogger<PublicModule> _logger { get; set; }
        //You can inject the host too. This is not generally recommended, but useful if you want to shutdown the host via a command.
        public IHost _host { get; set; }

        [Command("ping")]
        [Alias("pong", "hello")]
        public async Task PingAsync()
        {
            _logger.LogInformation($"User {Context.User.Username} used the ping command!");
            await ReplyAsync("pong!");
        }

        [Command("shutdown")]
        public async Task Stop()
        {
            //Don't do this if you're using the reliability extension, as it'll just restart the bot.
            _ = _host.StopAsync();
            //Instead do this
            //_ = _host.StopReliablyAsync();
        }

        [Command("log")]
        public async Task TestLogs()
        {
            _logger.LogTrace("This is a trace log");
            _logger.LogDebug("This is a debug log");
            _logger.LogInformation("This is an information log");
            _logger.LogWarning("This is a warning log");
            _logger.LogError(new InvalidOperationException("Invalid Operation"), "This is a error log with exception");
            _logger.LogCritical(new InvalidOperationException("Invalid Operation"), "This is a critical load with exception");

            _logger.Log(GetLogLevel(LogSeverity.Error), "Error logged from a Discord LogSeverity.Error");
            _logger.Log(GetLogLevel(LogSeverity.Info), "Information logged from Discord LogSeverity.Info ");
        }

        private static LogLevel GetLogLevel(LogSeverity severity)
            => (LogLevel)(Math.Abs((int)severity - 5));
    }
}
