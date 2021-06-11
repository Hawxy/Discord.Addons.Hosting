using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Sample.Serilog
{
    class Program
    {
        static async Task Main()
        {
            //Log is available everywhere, useful for places where it isn't practical to use ILogger injection
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .WriteTo.Console()
                .CreateLogger();


            //CreateDefaultBuilder configures a lot of stuff for us automatically.
            //See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0#default-builder-settings
            var hostBuilder = Host.CreateDefaultBuilder()
                //Serilog.Extensions.Hosting is required. Don't use ConfigureLogging to add Serilog.
                .UseSerilog()
                .ConfigureDiscordHost((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose,
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200
                    };

                    config.Token = context.Configuration["token"];

                    //Use this to configure a custom format for Client/CommandService logging if needed. The default is below and should be suitable for Serilog usage
                    config.LogFormat = (message, exception) => $"{message.Source}: {message.Message}";
                })
                //Omit this if you don't use the command service
                .UseCommandService((context, config) =>
                {
                    config.LogLevel = LogSeverity.Verbose;
                    config.DefaultRunMode = RunMode.Async;
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<CommandHandler>();
                });

            await hostBuilder.RunConsoleAsync();
        }
    }
}
