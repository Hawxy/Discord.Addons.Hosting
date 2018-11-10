using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Reliability;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SampleBotSerilog
{
    class Program
    {
        static async Task Main()
        {
            //Log is available everywhere, useful for places where it isn't practical to use ILogger injection
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            var builder = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    //Token is always loaded via app configuration "token" field
                    //See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/ for configuration source options
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("config.json", false, true)
                        .Build();
                    x.AddConfiguration(configuration);
                })
                //Serilog.Extensions.Hosting is required. Don't use ConfigureLogging to add Serilog.
                .UseSerilog()
                .ConfigureDiscordClient<DiscordSocketClient>((context, discordBuilder) =>
                {
                    var config = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose,
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200
                    };

                    discordBuilder.UseDiscordConfiguration(config);

                    //Optional pattern 
                    //var client = new DiscordSocketClient(config);
                    //discordBuilder.AddDiscordClient(client);
                })
                //Omit this if you don't use the command service
                .UseCommandService((context, config) =>
                {
                    config.LogLevel = LogSeverity.Verbose;
                    config.DefaultRunMode = RunMode.Async;
                })
                //Use this to configure a custom format for Client/CommandService logging if needed. The default is below and should be suitable for most people.
                .ConfigureDiscordLogFormat((message, exception) => $"{message.Source}: {message.Exception?.ToString() ?? message.Message}")
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<CommandHandler>();
                })
                .UseConsoleLifetime();

            //Start and stop just by hitting enter
            //See https://github.com/aspnet/Hosting/tree/master/samples/GenericHostSample for other control patterns
            var host = builder.Build();
            using (host)
            {
                await host.Services.GetRequiredService<CommandHandler>().InitializeAsync();

                while (true)
                {
                    Log.Information("Starting!");
                    await host.StartAsync();
                    Log.Information("Started! Press <enter> to stop.");
                    Console.ReadLine();

                    Log.Information("Stopping!");
                    await host.StopAsync();
                    Log.Information("Stopped! Press <enter> to start");
                    Console.ReadLine();
                }
            }
        }
    }
}
