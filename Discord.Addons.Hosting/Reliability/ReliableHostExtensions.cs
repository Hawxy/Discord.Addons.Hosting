#region License
/*
   Copyright 2019 Hawxy

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
#endregion
using System;
using System.Threading;
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
        private static ReliableDiscordHost? _reliable;
        private static CancellationTokenSource? _cts;

        /// <summary>
        /// Adds the Reliability Service and Runs the host. This function will only return if <see cref="StopReliablyAsync"/> is called elsewhere.
        /// </summary>
        /// <param name="host">The host to configure.</param>
        public static async Task RunReliablyAsync(this IHost host)
        {
            await host.WithReliability().StartAsync();
            _cts = new CancellationTokenSource();

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                _ = host.StopReliablyAsync();
            };

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                _ = host.StopReliablyAsync();
            };


            await Task.Delay(-1, _cts.Token).ContinueWith(_ => { });
        }

        /// <summary>
        /// FOR ADVANCED USE ONLY: Directly adds the reliability service to the host. This may result in unexpected behaviour. For most situations you should use <see cref="RunReliablyAsync"/> instead
        /// </summary>
        /// <param name="host">The host to configure.</param>
        internal static IHost WithReliability(this IHost host)
        {
            if(_reliable != null)
                throw new InvalidOperationException("Cannot add Reliability Host, it already exists!");

            var discord = host.Services.GetRequiredService<DiscordSocketClient>();
            var logger = host.Services.GetRequiredService<ILogger<ReliableDiscordHost>>();
            _reliable = new ReliableDiscordHost(discord, logger, host);
            
            return host;
        }

        /// <summary>
        /// Disposes the reliability service and stops the host. For use when <see cref="RunReliablyAsync"/> is used to start the host.
        /// </summary>
        /// <param name="host">The host to configure.</param>
        public static async Task StopReliablyAsync(this IHost host)
        {
            if (_reliable == null)
                throw new InvalidOperationException("Reliable host is null. The host may not be running or you didn't start it with RunReliablyAsync()");
            _reliable.Dispose();
            await host.StopAsync().ContinueWith(_ => 
            {
                _cts?.Cancel();
                _cts?.Dispose();
            });
        }
     }
}
