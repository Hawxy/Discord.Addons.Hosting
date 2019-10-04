#region License
/*
   Copyright 2019 Hawxy

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
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Discord.Addons.Hosting
{
    /// <summary>
    /// Extends <see cref="IHostBuilder"/> with Discord.Net configuration methods.
    /// </summary>
    public static class DiscordHostBuilderExtensions
    {
        /// <summary>
        /// Adds and configures a Discord.Net hosted instance of type <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// A <see cref="HostBuilderContext"/> is supplied so that the configuration and service provider can be used.
        /// </remarks>
        /// <typeparam name="T">The type of Discord.Net client. Type must be or inherit from <see cref="DiscordSocketClient"/></typeparam>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="config">The delegate for the <see cref="IDiscordHostConfigurationBuilder" /> that will be used to construct the discord client.</param>
        /// <returns>The generic host builder.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="config"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if client is already added to the service collection</exception>
        public static IHostBuilder ConfigureDiscordHost<T>(this IHostBuilder builder, Action<HostBuilderContext, IDiscordHostConfigurationBuilder> config) where T : DiscordSocketClient
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            builder.ConfigureServices((context, collection) =>
            {
                if (collection.Any(x => x.ServiceType == typeof(DiscordSocketClient)))
                    throw new InvalidOperationException($"Cannot add more than one Discord Client to host");
                
                var dschostbuilder = new DiscordHostConfigurationBuilder();
                config(context, dschostbuilder);

                var dsconfig = dschostbuilder.Build();

                collection.AddSingleton(dsconfig);

                var discordClient = (T)Activator.CreateInstance(typeof(T), dsconfig.SocketConfig);

                collection.AddSingleton(discordClient);
                if(typeof(T) != typeof(DiscordSocketClient))
                    collection.AddSingleton<DiscordSocketClient>(x => x.GetRequiredService<T>());

                collection.AddSingleton(typeof(LogAdapter<>));
                collection.AddHostedService<DiscordHostedService>();
            });

            return builder;

        }

        /// <summary>
        /// Adds a <see cref="CommandService"/> instance to the host for use with a Discord.Net client. />
        /// </summary>
        /// <param name="builder">The host builder to configure.</param> 
        /// <returns>The (generic) host builder.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="CommandService"/> is already added to the collection</exception>
        public static IHostBuilder UseCommandService(this IHostBuilder builder)
        {
            builder.ConfigureServices((context, collection) =>
            {
                if (collection.Any(x => x.ServiceType == typeof(CommandService)))
                    throw new InvalidOperationException("Cannot add more than one CommandService to host");
                collection.AddSingleton<CommandService>();
                collection.AddHostedService<CommandServiceRegistrationHost>();
            });
            return builder;
        }

        /// <summary>
        /// Adds a <see cref="CommandService"/> instance to the host for use with a Discord.Net client. />
        /// </summary>
        /// <remarks>
        /// A <see cref="HostBuilderContext"/> is supplied so that the configuration and service provider can be used.
        /// </remarks>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="config">The delegate for configuring the <see cref="CommandServiceConfig" /> that will be used to initialise the service.</param>
        /// <returns>The (generic) host builder.</returns>
        /// <exception cref="ArgumentNullException">Thrown if config is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="CommandService"/> is already added to the collection</exception>
        public static IHostBuilder UseCommandService(this IHostBuilder builder, Action<HostBuilderContext, CommandServiceConfig> config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            builder.ConfigureServices((context, collection) =>
            {
                if (collection.Any(x => x.ServiceType == typeof(CommandService)))
                    throw new InvalidOperationException("Cannot add more than one CommandService to host");

                var csc = new CommandServiceConfig();
                config(context, csc);
                collection.AddSingleton(new CommandService(csc));
                collection.AddHostedService<CommandServiceRegistrationHost>();

            });

            return builder;
        }
    }
}
