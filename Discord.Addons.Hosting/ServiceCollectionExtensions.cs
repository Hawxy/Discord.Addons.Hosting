#region License
/*
   Copyright 2019-2024 Hawxy

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

namespace Discord.Addons.Hosting;

/// <summary>
/// Extensions for registering Discord.NET services with <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
     /// <summary>
    /// Adds and optionally configures a <see cref="DiscordShardedClient"/> along with the required services.
    /// </summary>
    /// <remarks>
    /// A <see cref="IServiceProvider"/> is supplied, so you can pull data from additional services if required.
    /// </remarks>
    /// <param name="collection">The service collection to configure.</param>
    /// <param name="config">The delegate for the <see cref="DiscordHostConfiguration" /> that will be used to configure the host.</param>
    /// <exception cref="InvalidOperationException">Thrown if client is already added to the service collection</exception>
    public static IServiceCollection AddDiscordShardedHost(this IServiceCollection collection, Action<DiscordHostConfiguration, IServiceProvider> config)
    {
        collection.AddDiscordHostInternal<DiscordShardedClient>(config);

        if (collection.Any(x => x.ServiceType.BaseType == typeof(BaseSocketClient)))
            throw new InvalidOperationException("Cannot add more than one Discord Client to host");

        collection.AddSingleton<DiscordShardedClient, InjectableDiscordShardedClient>();

        return collection;
    }

    /// <summary>
    /// Adds and optionally configures a <see cref="DiscordSocketClient"/> along with the required services.
    /// </summary>
    /// <remarks>
    /// A <see cref="IServiceProvider"/> is supplied, so you can pull data from additional services if required.
    /// </remarks>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="config">The delegate for the <see cref="DiscordHostConfiguration" /> that will be used to configure the host.</param>
    /// <exception cref="InvalidOperationException">Thrown if client is already added to the service collection</exception>
    public static IServiceCollection AddDiscordHost(this IServiceCollection builder, Action<DiscordHostConfiguration, IServiceProvider> config)
    {
        builder.AddDiscordHostInternal<DiscordSocketClient>(config);

        if (builder.Any(x => x.ServiceType.BaseType == typeof(BaseSocketClient)))
            throw new InvalidOperationException("Cannot add more than one Discord Client to host");

        builder.AddSingleton<DiscordSocketClient, InjectableDiscordSocketClient>();

        return builder;
    }

    private static void AddDiscordHostInternal<T>(this IServiceCollection collection, Action<DiscordHostConfiguration, IServiceProvider> config) where T: BaseSocketClient
    {
        collection.AddOptions<DiscordHostConfiguration>()
            .Configure(config)
            .Validate(x => ValidateToken(x.Token), "Provided bot token is invalid or missing");

        collection.AddSingleton(typeof(LogAdapter<>));
        collection.AddHostedService<DiscordHostedService<T>>();

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
    /// <param name="collection">The service collection to configure.</param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="CommandService"/> is already added to the collection</exception>
    public static IServiceCollection AddCommandService(this IServiceCollection collection) => collection.AddCommandService((context, config) => { });

    /// <summary>
    /// Adds a <see cref="CommandService"/> instance to the host for use with a Discord.NET client. />
    /// </summary>
    /// <remarks>
    /// A <see cref="IServiceProvider"/> is supplied, so you can pull data from additional services if required.
    /// </remarks>
    /// <param name="collection">The service collection to configure.</param>
    /// <param name="config">The delegate for configuring the <see cref="CommandServiceConfig" /> that will be used to initialise the service.</param>
    /// <exception cref="ArgumentNullException">Thrown if config is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="CommandService"/> is already added to the collection</exception>
    public static IServiceCollection AddCommandService(this IServiceCollection collection, Action<CommandServiceConfig, IServiceProvider> config)
    {
        ArgumentNullException.ThrowIfNull(config);

        if (collection.Any(x => x.ServiceType == typeof(CommandService)))
            throw new InvalidOperationException("Cannot add more than one CommandService to host");

        collection.AddOptions<CommandServiceConfig>().Configure(config);

        collection.AddSingleton<CommandService, InjectableCommandService>();
        collection.AddHostedService<CommandServiceRegistrationHost>();

        return collection;
    }

    /// <summary>
    /// Adds a <see cref="InteractionService"/> instance to the host for use with a Discord.NET client. />
    /// </summary>
    /// <param name="collection">The service collection to configure.</param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="InteractionService"/> is already added to the collection</exception>
    public static IServiceCollection AddInteractionService(this IServiceCollection collection) => collection.AddInteractionService((_, _) => { });


    /// <summary>
    /// Adds a <see cref="InteractionService"/> instance to the host for use with a Discord.NET client. />
    /// </summary>
    /// <remarks>
    /// A <see cref="IServiceProvider"/> is supplied, so you can pull data from additional services if required.
    /// </remarks>
    /// <param name="collection">The service collection to configure.</param>
    /// <param name="config">The delegate for configuring the <see cref="InteractionServiceConfig" /> that will be used to initialise the service.</param>
    /// <exception cref="ArgumentNullException">Thrown if config is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="InteractionService"/> is already added to the collection</exception>
    public static IServiceCollection AddInteractionService(this IServiceCollection collection, Action<InteractionServiceConfig, IServiceProvider> config)
    {
        ArgumentNullException.ThrowIfNull(config);

        if (collection.Any(x => x.ServiceType == typeof(InteractionService)))
            throw new InvalidOperationException("Cannot add more than one InteractionService to host");

        collection.AddOptions<InteractionServiceConfig>().Configure(config);

        collection.AddSingleton<InteractionService, InjectableInteractionService>();
        collection.AddHostedService<InteractionServiceRegistrationHost>();

        return collection;
    }
}
