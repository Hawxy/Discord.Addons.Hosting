using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Sample.Web.ASP;

namespace Sample.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //I think it makes more sense to configure it here instead of modifying the default CreateHostBuilder
            //In a future release I might add separate DI extensions so you use ConfigureServices instead.
            CreateHostBuilder(args)
                .ConfigureDiscordClient<DiscordSocketClient>((context, discordBuilder) =>
                {
                    discordBuilder.UseDiscordConfiguration(new DiscordSocketConfig() { AlwaysDownloadUsers = true });
                })
                .UseCommandService((context, config) =>
                {
                    config.LogLevel = LogSeverity.Verbose;
                    config.DefaultRunMode = RunMode.Async;
                })
                .ConfigureDiscordLogFormat((message, exception) => message.ToString())
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
