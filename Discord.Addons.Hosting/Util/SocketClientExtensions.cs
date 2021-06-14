#region License
/*
   Copyright 2021 Hawxy

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

namespace Discord.Addons.Hosting.Util
{
    /// <summary>
    /// Utilities for the Discord.NET socket client
    /// </summary>
    public static class SocketClientExtensions
    {
        /// <summary>
        /// Asynchronously waits for the socket client's ready event to fire.
        /// </summary>
        /// <param name="client">The Discord.NET socket client</param>
        /// <param name="cancellationToken">The cancellation</param>
        /// <returns></returns>
        public static Task WaitForReadyAsync(this DiscordSocketClient client, CancellationToken cancellationToken)
        {
            if (_socketTcs is null)
                throw new InvalidOperationException("The socket client has not been registered correctly. Did you use ConfigureDiscordHost on your HostBuilder?");

            if(_socketTcs.Task.IsCompleted)
                return _socketTcs.Task;

            var registration = cancellationToken.Register(
                state => { ((TaskCompletionSource<object>)state!).TrySetResult(null!); },
                _socketTcs);

            return _socketTcs.Task.ContinueWith(_=> registration.DisposeAsync());
        }

        private static TaskCompletionSource<object>? _socketTcs;
        internal static void RegisterSocketClientReady(this DiscordSocketClient client)
        {
            _socketTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            client.Ready += ClientReady;

            Task ClientReady()
            {
                _socketTcs!.TrySetResult(null!);
                client.Ready -= ClientReady;
                return Task.CompletedTask;
            }
        }
    }
}
