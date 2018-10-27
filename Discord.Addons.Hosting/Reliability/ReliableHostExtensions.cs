using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.Hosting.Reliability
{
    public static class ReliableHostExtensions 
    {
        public static async Task RunAndBlockReliablyAsync<T>(this IHost host) where T : DiscordSocketClient, new()
        {
            host.WithReliability<T>();
            await host.StartAsync();
            await Task.Delay(-1);
        }
        public static IHost WithReliability<T>(this IHost host) where T: DiscordSocketClient, new()
        {
            var lifetime = host.Services.GetService<IApplicationLifetime>();
            var discord = host.Services.GetService<T>();
            var logger = host.Services.GetService<ILogger<ReliableDiscordHost>>();
            var runner = new ReliableDiscordHost(discord, logger, host);
            lifetime.ApplicationStopping.Register(() => runner.Dispose());
            
            return host;
        }
    }
}
