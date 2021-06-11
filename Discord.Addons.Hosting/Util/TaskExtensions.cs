using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Addons.Hosting.Util
{
    internal static class TaskExtensions
    {
        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            await using (cancellationToken.Register(state =>
                {
                    ((TaskCompletionSource<object>)state!).TrySetResult(null!);
                },
                tcs))
            {
                var resultTask = await Task.WhenAny(task, tcs.Task);
                if (resultTask == tcs.Task)
                {
                   throw new OperationCanceledException(cancellationToken);
                }
            }
        }
    }


}
