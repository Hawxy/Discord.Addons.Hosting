#region License
/*
   Copyright 2019-2022 Hawxy

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

using Discord.WebSocket;

namespace Discord.Addons.Hosting.Util
{
    /// <summary>
    /// Utilities for the Discord.NET sharded client
    /// </summary>
    public static class ShardedClientExtensions
    {
        /// <summary>
        /// Asynchronously waits for all shards to report ready.
        /// </summary>
        /// <param name="client">The Discord.NET sharded client.</param>
        /// <param name="cancellationToken">A cancellation token that will cause an early exit if cancelled.</param>
        /// <returns></returns>
        public static Task WaitForReadyAsync(this DiscordShardedClient client, CancellationToken cancellationToken)
        {
            if (_shardedTcs is null)
                throw new InvalidOperationException("The sharded client has not been registered correctly. Did you use ConfigureDiscordShardedHost on your HostBuilder?");

            if (_shardedTcs.Task.IsCompleted)
                return _shardedTcs.Task;

            var registration = cancellationToken.Register(
                state => { ((TaskCompletionSource<object>)state!).TrySetResult(null!); },
                _shardedTcs);

            return _shardedTcs.Task.ContinueWith(_ => registration.DisposeAsync());
        }

        private static TaskCompletionSource<object>? _shardedTcs;

        internal static void RegisterShardedClientReady(this DiscordShardedClient client)
        {
            _shardedTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            var shardReadyCount = 0;

            client.ShardReady += ShardReady;

            Task ShardReady(DiscordSocketClient _)
            {
                shardReadyCount++;
                if (shardReadyCount == client.Shards.Count)
                {
                    _shardedTcs!.TrySetResult(null!);
                    client.ShardReady -= ShardReady;
                }
                return Task.CompletedTask;
            }

        }
    }
}
