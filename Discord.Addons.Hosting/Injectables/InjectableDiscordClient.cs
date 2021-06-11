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
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Discord.Addons.Hosting.Injectables
{
    internal class InjectableDiscordSocketClient : DiscordSocketClient
    {
        public InjectableDiscordSocketClient(IOptions<DiscordHostConfiguration> config) : base(config.Value.SocketConfig)
        {
        }
    }

    internal class InjectableDiscordShardedClient : DiscordShardedClient
    {
        public InjectableDiscordShardedClient(IOptions<DiscordHostConfiguration> config) : base(config.Value.SocketConfig)
        {
        }
    }
}
