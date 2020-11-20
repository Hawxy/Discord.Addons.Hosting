#region License
/*
   Copyright 2020 Hawxy

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
using Discord.Addons.Hosting.Util;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Discord.Addons.Hosting
{
    /// <summary>
    /// Extends <see cref="IHostBuilder"/> with Discord.Net configuration methods.
    /// </summary>
    public static class DiscordHostBuilderExtensions
    {
        /// <summary>
        /// Adds and optionally configures a Discord.Net hosted instance of type <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// A <see cref="HostBuilderContext"/> is supplied so that the configuration and service provider can be used.
        /// </remarks>
        /// <typeparam name="T">The type of Discord.Net client. Type must be or inherit from <see cref="DiscordSocketClient"/></typeparam>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="config">The delegate for the <see cref="DiscordHostConfiguration" /> that will be used to configure the host.</param>
        /// <returns>The generic host builder.</returns>
        /// <exception cref="InvalidOperationException">Thrown if client is already added to the service collection</exception>
        public static IHostBuilder ConfigureDiscordHost<T>(this IHostBuilder builder, Action<HostBuilderContext, DiscordHostConfiguration>? config = null) where T : DiscordSocketClient
        {
            builder.ConfigureServices((context, collection) =>
            {
                if (collection.Any(x => x.ServiceType == typeof(DiscordSocketClient)))
                    throw new InvalidOperationException($"Cannot add more than one Discord Client to host");

                collection.AddOptions<DiscordHostConfiguration>().Validate(x => ValidateToken(x.Token));

                if (config != null)
                    collection.Configure<DiscordHostConfiguration>(x => config(context, x));

                collection.AddSingleton(x=> (T)Activator.CreateInstance(typeof(T), x.GetRequiredService<IOptions<DiscordHostConfiguration>>().Value.SocketConfig)!);
                if(typeof(T) != typeof(DiscordSocketClient))
                    collection.AddSingleton<DiscordSocketClient>(x => x.GetRequiredService<T>());

                collection.AddSingleton(typeof(LogAdapter<>));
                collection.AddHostedService<DiscordHostedService>();
            });

            return builder;

            static bool ValidateToken(string token)
            {
                try
                {
                    TokenUtils.ValidateToken(TokenType.Bot, token);
                    return true;
                }
                catch (Exception e) when (e is ArgumentNullException || e is ArgumentException)
                {
                    return false;
                }
            }

        }

        /// <summary>
        /// Adds a <see cref="CommandService"/> instance to the host for use with a Discord.Net client. />
        /// </summary>
        /// <param name="builder">The host builder to configure.</param> 
        /// <returns>The (generic) host builder.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="CommandService"/> is already added to the collection</exception>
        public static IHostBuilder UseCommandService(this IHostBuilder builder) => builder.UseCommandService((context, config) => { });

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

                collection.Configure<CommandServiceConfig>(x => config(context, x));

                collection.AddSingleton(x=> new CommandService(x.GetRequiredService<IOptions<CommandServiceConfig>>().Value));
                collection.AddHostedService<CommandServiceInitializerHost>();
            });

            return builder;
        }
    }
}
