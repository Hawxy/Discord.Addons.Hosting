#region License
/*
   Copyright 2018 Hawxy

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
using Discord.WebSocket;

namespace Discord.Addons.Hosting
{
    /// <summary>
    /// Simple handler to manage client creation
    /// <typeparam name="T">The type of Discord.Net client. Type must be or inherit from <see cref="DiscordSocketClient"/></typeparam>
    /// </summary>
    public class DiscordClientHandler<T> where T: DiscordSocketClient
    {
        private T _client;

        /// <summary>
        /// Adds a Discord client to the handler. Do not use in combination with <see cref="UseDiscordConfiguration{Y}"/>
        /// </summary>
        /// <param name="client">A Discord client instance</param>
        /// <exception cref="InvalidOperationException">Thrown if client initialized more than once</exception>
        public void AddDiscordClient(T client)
        {
            if (_client != null)
                throw new InvalidOperationException("Client can only be initialized once!");
            _client = client;
        }

        /// <summary>
        /// Creates a discord client with the provided configuration. Do not use in combination with <see cref="AddDiscordClient"/>
        /// </summary>
        /// <param name="config">The Discord client configuration object. Ensure the type is compatible with the client type <typeparamref name="T"/></param>
        /// <exception cref="InvalidOperationException">Thrown if client initialized more than once</exception>
        public void UseDiscordConfiguration<Y>(Y config) where Y : DiscordSocketConfig
        {
            if(_client != null)
                throw new InvalidOperationException("Client can only be initialized once!");
      
            _client = (T)Activator.CreateInstance(typeof(T), config);
        }

        /// <summary>
        /// Returns the Discord client object
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if client is null</exception>
        public T GetClient()
        {
            if(_client == null)
                throw new InvalidOperationException("Client cannot be null");
            return _client;
        }
    }
}
