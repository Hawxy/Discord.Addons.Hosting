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

using Discord.Addons.Hosting.Util;
using Discord.Interactions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.Hosting.Services
{
    internal class InteractionServiceRegistrationHost : IHostedService
    {
        private readonly InteractionService _interactionService;
        private readonly ILogger<InteractionServiceRegistrationHost> _logger;
        private readonly LogAdapter<InteractionService> _adapter;

        public InteractionServiceRegistrationHost(InteractionService interactionService, ILogger<InteractionServiceRegistrationHost> logger, LogAdapter<InteractionService> adapter)
        {
            _interactionService = interactionService;
            _logger = logger;
            _adapter = adapter;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _interactionService.Log += _adapter.Log;
            _logger.LogInformation($"Registered logger for {nameof(InteractionService)}");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    }
}
