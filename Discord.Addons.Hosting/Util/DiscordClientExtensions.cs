using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Discord.Addons.Hosting.Util
{
    public static class DiscordClientExtensions
    {
        /// <summary>
        /// Asynchronously waits for the client's ready event to fire.
        /// </summary>
        /// <param name="client">The Discord.NET socket client</param>
        /// <param name="cancellationToken">The cancellation</param>
        /// <returns></returns>
        public static Task WaitForReadyAsync(this DiscordSocketClient client, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            var registration = cancellationToken.Register(
                state => { ((TaskCompletionSource<object>)state!).TrySetResult(null!); },
                tcs);
            
            client.Ready += ClientReady;

            Task ClientReady()
            {
                tcs.TrySetResult(null!);
                client.Ready -= ClientReady;
                return Task.CompletedTask;
            }

            return tcs.Task.ContinueWith(_=> registration.DisposeAsync());
        }

       /* private static TaskCompletionSource<object> _tcs =
            new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

        internal static void RegisterSocketClientReady(this DiscordSocketClient client)
        {

        }*/
    }

    
}
