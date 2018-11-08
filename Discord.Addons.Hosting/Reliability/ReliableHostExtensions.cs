﻿using System;
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
        private static ReliableDiscordHost _reliable;

        /// <summary>
        /// Adds the Reliability Service and Runs the host. This function will never return. Do not use in combination with <see cref="WithReliability{T}"/>
        /// </summary>
        /// <param name="host">The host to configure.</param>
        public static async Task RunReliablyAsync(this IHost host)
        {
            host.WithReliability();
            await host.StartAsync();
            await Task.Delay(-1);
        }
        /// <summary>
        /// FOR ADVANCED USE ONLY: Directly adds the reliability service to the host. This may result in unexpected behaviour. For most situations you should use <see cref="RunReliablyAsync"/> instead
        /// </summary>
        /// <param name="host">The host to configure.</param>
        public static IHost WithReliability(this IHost host)
        {
            if(_reliable != null)
                throw new InvalidOperationException("Cannot add Reliability Host, it already exists!");

            var discord = host.Services.GetRequiredService<DiscordSocketClient>();
            var logger = host.Services.GetRequiredService<ILogger<ReliableDiscordHost>>();
            _reliable = new ReliableDiscordHost(discord, logger, host);
            
            return host;
        }

        //public static async Task QuitReliablyAsync(this IHost host)
        //{
        //    if (_reliable == null)
        //        throw new InvalidOperationException("Reliable host is null. Shutdown the host normally with StopAsync instead.");
        //    _reliable.Dispose();
        //    await host.StopAsync();
        //    Environment.Exit(0);
        //}
    }
}
