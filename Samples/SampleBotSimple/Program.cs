using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sample.Simple
{
    class Program
    {
        static async Task Main()
        {
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
                .ConfigureLogging(x =>
                {
                    //The default console logger doesn't have a great format, I recommend using a third-party one as is shown in the Serilog example
                    x.AddConsole();
                    x.SetMinimumLevel(LogLevel.Debug);
                })
                //Specify the type of discord.net client via the type parameter
                .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose,
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200
                    };

                    config.Token = context.Configuration["token"];
                })
                //Omit this if you don't use the command service
                .UseCommandService()
                .ConfigureServices((context, services) =>
                {
                    //Add any other services here
                    services.AddHostedService<CommandHandler>();
                })
                .UseConsoleLifetime();
            
            var host = builder.Build();
            using (host)
            {
                //Fire and forget. Will run until console is closed or the service is stopped. Basically the same as normally running the bot.
                await host.RunAsync();
            }
        }
    }
}
