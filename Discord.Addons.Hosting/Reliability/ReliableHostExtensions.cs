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
        /// <typeparam name="T">The type of Discord.Net client being run. Type must inherit from <see cref="DiscordSocketClient"/></typeparam>
        /// <param name="host">The host to configure.</param>
        public static async Task RunAndBlockReliablyAsync<T>(this IHost host) where T : DiscordSocketClient, new()
        {
            host.WithReliability<T>();
            await host.StartAsync();
            await Task.Delay(-1);
        }
        /// <summary>
        /// FOR ADVANCED USE ONLY: Manually adds the reliability service to the host. This may result in unexpected behaviour. For most situations you should use <see cref="RunAndBlockReliablyAsync{T}"/> instead
        /// </summary>
        /// <typeparam name="T">The type of Discord.Net client being run. Type must inherit from <see cref="DiscordSocketClient"/></typeparam>
        /// <param name="host">The host to configure.</param>
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
