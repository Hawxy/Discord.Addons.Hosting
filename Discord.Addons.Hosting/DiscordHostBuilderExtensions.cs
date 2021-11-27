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

using Discord.Addons.Hosting.Injectables;
using Discord.Addons.Hosting.Services;
using Discord.Addons.Hosting.Util;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Discord.Addons.Hosting;

/// <summary>
/// Extends <see cref="IHostBuilder"/> with Discord.Net configuration methods.
/// </summary>
public static class DiscordHostBuilderExtensions
{
    /// <summary>
    /// Adds and optionally configures a <see cref="DiscordShardedClient"/> along with the required services.
    /// </summary>
    /// <remarks>
    /// A <see cref="HostBuilderContext"/> is supplied so that the configuration and service provider can be used.
    /// </remarks>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="config">The delegate for the <see cref="DiscordHostConfiguration" /> that will be used to configure the host.</param>
    /// <returns>The generic host builder.</returns>
    /// <exception cref="InvalidOperationException">Thrown if client is already added to the service collection</exception>
    public static IHostBuilder ConfigureDiscordShardedHost(this IHostBuilder builder, Action<HostBuilderContext, DiscordHostConfiguration>? config = null)
    {
        builder.ConfigureDiscordHostInternal<DiscordShardedClient>(config);

        return builder.ConfigureServices((_, collection) =>
        {
            if (collection.Any(x => x.ServiceType.BaseType == typeof(BaseSocketClient)))
                throw new InvalidOperationException("Cannot add more than one Discord Client to host");

            collection.AddSingleton<DiscordShardedClient, InjectableDiscordShardedClient>();
        });
    }

    /// <summary>
    /// Adds and optionally configures a <see cref="DiscordSocketClient"/> along with the required services.
    /// </summary>
    /// <remarks>
    /// A <see cref="HostBuilderContext"/> is supplied so that the configuration and service provider can be used.
    /// </remarks>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="config">The delegate for the <see cref="DiscordHostConfiguration" /> that will be used to configure the host.</param>
    /// <returns>The generic host builder.</returns>
    /// <exception cref="InvalidOperationException">Thrown if client is already added to the service collection</exception>
    public static IHostBuilder ConfigureDiscordHost(this IHostBuilder builder, Action<HostBuilderContext, DiscordHostConfiguration>? config = null)
    {
        builder.ConfigureDiscordHostInternal<DiscordSocketClient>(config);

        return builder.ConfigureServices((_, collection) =>
        {
            if (collection.Any(x => x.ServiceType.BaseType == typeof(BaseSocketClient)))
                throw new InvalidOperationException("Cannot add more than one Discord Client to host");

            collection.AddSingleton<DiscordSocketClient, InjectableDiscordSocketClient>();
        });
    }

    private static IHostBuilder ConfigureDiscordHostInternal<T>(this IHostBuilder builder, Action<HostBuilderContext, DiscordHostConfiguration>? config = null) where T: BaseSocketClient
    {
        return builder.ConfigureServices((context, collection) =>
        {
            collection.AddOptions<DiscordHostConfiguration>().Validate(x => ValidateToken(x.Token));

            if (config != null)
                collection.Configure<DiscordHostConfiguration>(x => config(context, x));
                
            collection.AddSingleton(typeof(LogAdapter<>));
            collection.AddHostedService<DiscordHostedService<T>>();
        });

        static bool ValidateToken(string token)
        {
            try
            {
                TokenUtils.ValidateToken(TokenType.Bot, token);
                return true;
            }
            catch (Exception e) when (e is ArgumentNullException or ArgumentException)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Adds a <see cref="CommandService"/> instance to the host for use with a Discord.NET client. />
    /// </summary>
    /// <param name="builder">The host builder to configure.</param> 
    /// <returns>The (generic) host builder.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="CommandService"/> is already added to the collection</exception>
    public static IHostBuilder UseCommandService(this IHostBuilder builder) => builder.UseCommandService((context, config) => { });

    /// <summary>
    /// Adds a <see cref="CommandService"/> instance to the host for use with a Discord.NET client. />
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
        ArgumentNullException.ThrowIfNull(config);

        builder.ConfigureServices((context, collection) =>
        {
            if (collection.Any(x => x.ServiceType == typeof(CommandService)))
                throw new InvalidOperationException("Cannot add more than one CommandService to host");

            collection.Configure<CommandServiceConfig>(x => config(context, x));

            collection.AddSingleton(x=> new CommandService(x.GetRequiredService<IOptions<CommandServiceConfig>>().Value));
            collection.AddHostedService<CommandServiceRegistrationHost>();
        });

        return builder;
    }

    /// <summary>
    /// Adds a <see cref="InteractionService"/> instance to the host for use with a Discord.NET client. />
    /// </summary>
    /// <param name="builder">The host builder to configure.</param> 
    /// <returns>The (generic) host builder.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="InteractionService"/> is already added to the collection</exception>
    public static IHostBuilder UseInteractionService(this IHostBuilder builder) => builder.UseCommandService((context, config) => { });


    /// <summary>
    /// Adds a <see cref="InteractionService"/> instance to the host for use with a Discord.NET client. />
    /// </summary>
    /// <remarks>
    /// A <see cref="HostBuilderContext"/> is supplied so that the configuration and service provider can be used.
    /// </remarks>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="config">The delegate for configuring the <see cref="InteractionServiceConfig" /> that will be used to initialise the service.</param>
    /// <returns>The (generic) host builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown if config is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="InteractionService"/> is already added to the collection</exception>
    public static IHostBuilder UseInteractionService(this IHostBuilder builder, Action<HostBuilderContext, InteractionServiceConfig> config)
    {
        ArgumentNullException.ThrowIfNull(config);

        builder.ConfigureServices((context, collection) =>
        {
            if (collection.Any(x => x.ServiceType == typeof(InteractionService)))
                throw new InvalidOperationException("Cannot add more than one InteractionService to host");

            collection.Configure<InteractionServiceConfig>(x => config(context, x));

            collection.AddSingleton<InteractionService, InjectableInteractionsService>();
            collection.AddHostedService<InteractionServiceRegistrationHost>();
        });

        return builder;
    }
}