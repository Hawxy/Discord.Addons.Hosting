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
    /// Useful utilities for Discord.NET client
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
            if (_tcs is null)
                throw new InvalidOperationException("The socket client has not been registered correctly. Did you use ConfigureDiscordHost in your HostBuilder?");

            if(_tcs.Task.IsCompleted)
                return _tcs.Task;

            var registration = cancellationToken.Register(
                state => { ((TaskCompletionSource<object>)state!).TrySetResult(null!); },
                _tcs);

            return _tcs.Task.ContinueWith(_=> registration.DisposeAsync());
        }

        private static TaskCompletionSource<object>? _tcs;
        internal static void RegisterSocketClientReady(this DiscordSocketClient client)
        {
            _tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            client.Ready += ClientReady;

            Task ClientReady()
            {
                _tcs!.TrySetResult(null!);
                client.Ready -= ClientReady;
                return Task.CompletedTask;
            }
        }
    }

    
}
