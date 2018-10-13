using System;
using Discord.WebSocket;

namespace Discord.Addons.Hosting
{
    /// <summary>
    /// Simple builder used to create a Discord Client
    /// <typeparam name="T">The type of Discord.Net client. Type must inherit from <see cref="BaseSocketClient"/></typeparam>
    /// </summary>
    public class DiscordBuilder<T> where T: BaseSocketClient, new()
    {
        private T _client;

        /// <summary>
        /// Adds a Discord client to the builder. Do not use in combination with <see cref="UseDiscordConfiguration{Y}"/>
        /// </summary>
        /// <param name="client">A Discord client instance</param>
        /// <returns>An instance of <see cref="DiscordBuilder{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if client initialized more than once</exception>
        public DiscordBuilder<T> AddDiscordClient(T client)
        {
            if (_client != null)
                throw new InvalidOperationException("Client can only be initialized once!");
            _client = client;
            return this;
        }

        /// <summary>
        /// Adds a Discord client configuration object to the builder. Do not use in combination with <see cref="AddDiscordClient"/>
        /// </summary>
        /// <param name="config">A Discord client configuration object. Ensure the type is compatible with <typeparamref name="T"/></param>
        /// <returns>An instance of <see cref="DiscordBuilder{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if client initialized more than once</exception>
        public DiscordBuilder<T> UseDiscordConfiguration<Y>(Y config) where Y : DiscordConfig
        {
            if(_client != null)
                throw new InvalidOperationException("Client can only be initialized once!");
            //has performance issues, but does it matter if it's only called once?
            _client = (T)Activator.CreateInstance(typeof(T), config);
            return this;
        }

        /// <summary>
        /// Returns the Discord client object
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if client is null</exception>
        public T Build()
        {
            if(_client == null)
                throw new InvalidOperationException("Client cannot be null");
            return _client;
        }
    }
}
