using System.IO;
using System.Threading.Tasks;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Reliability;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleBotSimple
{
    class Program
    {
        //Requires C# 7.1 or later
        static async Task Main()
        {
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
                .ConfigureLogging(x =>
                {
                    x.SetMinimumLevel(LogLevel.Information);
                    //This works but isn't very pretty. I would highly suggest using Serilog or some other third-party logger
                    //See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.2#built-in-logging-providers for more logging options
                    x.AddConsole();

                    //Inject ILogger in any services/modules that require logging

                })
                .ConfigureDiscordClient<DiscordSocketClient>((context, discordBuilder) =>
                {
                    var config = new DiscordSocketConfig();
                    discordBuilder.UseDiscordConfiguration(config);

                    //Optional pattern 
                    //var client = new DiscordSocketClient(config);
                    //discordBuilder.AddDiscordClient(client);
                })
                //Omit this if you don't use the command service
                .UseCommandService()
                //This format might be better if you're NOT using Serilog or any other third-party providers
                .ConfigureDiscordLogFormat((message, exception) => message.ToString())
                .ConfigureServices((context, services) =>
                {
                    //Add any other services here
                    services.AddSingleton<CommandHandler>();
                })
                .UseConsoleLifetime();
                

            var host = builder.Build();
            using (host)
            {
                await host.Services.GetRequiredService<CommandHandler>().InitializeAsync();
                //Fire and forget. Will run until console is closed or the service is stopped. Basically the same as normally running the bot.
                await host.RunAsync();
                //If you want the host to attempt a restart when the client fails to reconnect, use the Reliability extension.
                //await host.RunReliablyAsync();
            }
        }
    }
}
