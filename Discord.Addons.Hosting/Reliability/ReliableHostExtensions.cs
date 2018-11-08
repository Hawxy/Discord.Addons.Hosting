using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.Hosting.Reliability
{
    /// <summary>
    /// Extends <see cref="IHost"/> with Discord.Net Reliability options.
    /// </summary>
    public static class ReliableHostExtensions 
    {
        /// <summary>
        /// Adds the Reliability Service and Runs the host. This function will never return. Do not use in combination with <see cref="WithReliability{T}"/>
        /// </summary>
        /// <param name="host">The host to configure.</param>
        public static async Task RunAndBlockReliablyAsync(this IHost host)
        {
            host.WithReliability();
            await host.StartAsync();
            await Task.Delay(-1);
        }
        /// <summary>
        /// FOR ADVANCED USE ONLY: Manually adds the reliability service to the host. This may result in unexpected behaviour. For most situations you should use <see cref="RunAndBlockReliablyAsync{T}"/> instead
        /// </summary>
        /// <param name="host">The host to configure.</param>
        public static IHost WithReliability(this IHost host)
        {
            var lifetime = host.Services.GetService<IApplicationLifetime>();
            var discord = host.Services.GetService<DiscordSocketClient>();
            var logger = host.Services.GetService<ILogger<ReliableDiscordHost>>();
            var runner = new ReliableDiscordHost(discord, logger, host);
            lifetime.ApplicationStopping.Register(() => runner.Dispose());
            
            return host;
        }
    }
}
