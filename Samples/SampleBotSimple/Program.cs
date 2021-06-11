using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Sample.Simple
{
    class Program
    {
        static async Task Main()
        {
            //CreateDefaultBuilder configures a lot of stuff for us automatically.
            //See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0#default-builder-settings
            var hostBuilder = Host.CreateDefaultBuilder() 
                /*.ConfigureAppConfiguration(x =>
                {
                    // The hostbuilder will load your settings from appsettings.json by default
                    // You can add more configuration sources here if required.
                    // See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/ for configuration source options
                })
                .ConfigureLogging(x =>
                {
                    // You can configure logging here.
                    // The default console logger doesn't have a great format, I recommend using a third-party one as is shown in the Serilog example
                    x.SetMinimumLevel(LogLevel.Debug);
                })*/
                //Specify the type of discord.net client via the type parameter
                .ConfigureDiscordHost((context, config) =>
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
                .UseCommandService((context, config) =>
                {
                    config.DefaultRunMode = RunMode.Async;
                    config.CaseSensitiveCommands = false;
                })
                .ConfigureServices((context, services) =>
                {
                    //Add any other services here
                    services.AddHostedService<CommandHandler>();
                });
                

            await hostBuilder.RunConsoleAsync();
        }
    }
}
