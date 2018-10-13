using System;
using Discord.WebSocket;

namespace Discord.Addons.Hosting
{
    /// <summary>
    /// Simple handler to manage client creation
    /// <typeparam name="T">The type of Discord.Net client. Type must inherit from <see cref="BaseSocketClient"/></typeparam>
    /// </summary>
    public class DiscordClientHandler<T> where T: BaseSocketClient, new()
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
        /// <param name="config">A Discord client configuration object. Ensure the type is compatible with <typeparamref name="T"/></param>
        /// <exception cref="InvalidOperationException">Thrown if client initialized more than once</exception>
        public void UseDiscordConfiguration<Y>(Y config) where Y : DiscordConfig
        {
            if(_client != null)
                throw new InvalidOperationException("Client can only be initialized once!");
            //has performance issues, but does it matter if it's only called once?
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
