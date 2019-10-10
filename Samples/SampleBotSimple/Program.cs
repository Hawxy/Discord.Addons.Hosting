using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sample.Simple
{
    class Program
    {
        static async Task Main()
        {
            //Creates a builder with a number of defaults already set. See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0#default-builder-settings
            var builder = Host
                .CreateDefaultBuilder()
                .ConfigureLogging(x =>
                {
                    //The default builder adds a number of logging providers that aren't super useful for this scenario, so I recommend doing the below
                    x.ClearProviders();
                    //The default console logger doesn't have a great format, I recommend using a third-party one as is shown in the Serilog example
                    x.AddConsole();
                    x.SetMinimumLevel(LogLevel.Debug);
                })
                //Specify the type of discord.net client via the type parameter
                .ConfigureDiscordHost<DiscordSocketClient>((context, configurationBuilder) =>
                {
                    //The default builder will look for appsettings.json and any environment variables prefixed with "DOTNET_"
                    configurationBuilder.SetToken(context.Configuration["token"]);

                    configurationBuilder.SetDiscordConfiguration(new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose,
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200
                    });
                })
                //Omit this if you don't use the command service
                .UseCommandService()
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
