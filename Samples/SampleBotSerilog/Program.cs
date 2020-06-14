using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
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

            var builder = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    //See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/ for configuration source options
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true)
                        .Build();

                    x.AddConfiguration(configuration);
                })
                //Serilog.Extensions.Hosting is required. Don't use ConfigureLogging to add Serilog.
                .UseSerilog()
                .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
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
                })
                .UseConsoleLifetime();

            //Stop just by hitting enter
            //See https://github.com/aspnet/Extensions/tree/master/src/Hosting/samples/GenericHostSample for other control patterns
            var host = builder.Build();
            using (host)
            {
                Log.Information("Starting!");
                await host.RunAsync();
                Log.Information("Started! Press <enter> to stop.");
                Console.ReadLine();

                Log.Information("Stopping!");
                await host.StopAsync();
                Log.Information("Stopped!");
                Console.ReadLine();
            }
        }
    }
}
