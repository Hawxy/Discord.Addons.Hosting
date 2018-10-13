using System;
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
        /// <typeparam name="T">The type of Discord.Net client. Type must inherit from <see cref="BaseSocketClient"/></typeparam>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="config">The delegate for configuring the <see cref="DiscordClientHandler{T}" /> that will be used to construct the discord client.</param>
        /// <returns>The (generic) host builder.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="config"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if client is already logged in</exception>
        public static IHostBuilder ConfigureDiscordClient<T>(this IHostBuilder builder, Action<HostBuilderContext, DiscordClientHandler<T>> config) where T: BaseSocketClient, new()
        {
            if(config == null)
                throw new ArgumentNullException(nameof(config));

            builder.ConfigureServices((context, collection) =>
            {
                var token = context.Configuration["token"];
                TokenUtils.ValidateToken(TokenType.Bot, token);

                var dsc = new DiscordClientHandler<T>();
                config(context, dsc);
                var client = dsc.GetClient();

                if(client.LoginState != LoginState.LoggedOut)
                    throw new InvalidOperationException("Client logged in before host startup! Make sure you aren't calling LoginAsync manually");
                
                collection.AddSingleton(client);
                collection.AddHostedService<DiscordHostedService<T>>();
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
        /// <returns>The (generic) host builder.</returns>
        public static IHostBuilder UseCommandService(this IHostBuilder builder)
        {
            builder.ConfigureServices((context, collection) =>
            {
                collection.AddSingleton<CommandService>();
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
        public static IHostBuilder UseCommandService(this IHostBuilder builder, Action<HostBuilderContext, CommandServiceConfig> config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            builder.ConfigureServices((context, collection) =>
            {
                var csc = new CommandServiceConfig();
                config(context, csc);
                var service = new CommandService(csc);
                collection.AddSingleton(service);

            });

            return builder;
        }
    }
}
