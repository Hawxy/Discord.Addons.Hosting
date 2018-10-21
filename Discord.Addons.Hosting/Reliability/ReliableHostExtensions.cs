using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.Hosting.Reliability
{
    public static class ReliableHostExtensions 
    {
        public static IHost WithReliability<T>(this IHost host) where T: DiscordSocketClient, new()
        {
            var discord = host.Services.GetService<T>();
            var logger = host.Services.GetService<ILogger<ReliableDiscordHost>>();
            var runner = new ReliableDiscordHost(discord, logger, host);
            return host;
        }
    }
}
