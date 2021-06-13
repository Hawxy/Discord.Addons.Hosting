﻿#region License
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
