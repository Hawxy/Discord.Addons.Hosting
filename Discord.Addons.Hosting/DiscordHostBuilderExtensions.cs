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

using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Discord.Addons.Hosting;

/// <summary>
/// Extensions <see cref="IHostBuilder"/> with Discord.Net configuration options.
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
    [Obsolete("This extension is obsolete and will be removed in a future version. Replace with builder.Services.AddDiscordShardedHost. See the Discord.Addons.Hosting repository for more information.")]
    public static IHostBuilder ConfigureDiscordShardedHost(this IHostBuilder builder, Action<HostBuilderContext, DiscordHostConfiguration> config)
    {
        ArgumentNullException.ThrowIfNull(config);
        return builder.ConfigureServices((context, collection) =>
        {
            collection.AddDiscordShardedHost((hostConfig, _) => config(context, hostConfig));
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
    [Obsolete("This extension is obsolete and will be removed in a future version. Replace with builder.Services.AddDiscordHost. See the Discord.Addons.Hosting repository for more information.")]
    public static IHostBuilder ConfigureDiscordHost(this IHostBuilder builder, Action<HostBuilderContext, DiscordHostConfiguration> config)
    {
        ArgumentNullException.ThrowIfNull(config);
        return builder.ConfigureServices((context, collection) =>
        {
            collection.AddDiscordHost((hostConfig, _) => config(context, hostConfig));
        });
    }

    /// <summary>
    /// Adds a <see cref="CommandService"/> instance to the host for use with a Discord.NET client. />
    /// </summary>
    /// <param name="builder">The host builder to configure.</param> 
    /// <returns>The (generic) host builder.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="CommandService"/> is already added to the collection</exception>
    [Obsolete("This extension is obsolete and will be removed in a future version. Replace with builder.Services.AddCommandService. See the Discord.Addons.Hosting repository for more information.")]
    public static IHostBuilder UseCommandService(this IHostBuilder builder) =>
        builder.ConfigureServices((_, collection) => collection.AddCommandService());

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
    [Obsolete("This extension is obsolete and will be removed in a future version. Replace with builder.Services.AddCommandService. See the Discord.Addons.Hosting repository for more information.")]
    public static IHostBuilder UseCommandService(this IHostBuilder builder, Action<HostBuilderContext, CommandServiceConfig> config)
    {
        ArgumentNullException.ThrowIfNull(config);
        builder.ConfigureServices((context, collection) =>
        {
            collection.AddCommandService((commandServiceConfig, _) => config(context, commandServiceConfig));
        });

        return builder;
    }

    /// <summary>
    /// Adds a <see cref="InteractionService"/> instance to the host for use with a Discord.NET client. />
    /// </summary>
    /// <param name="builder">The host builder to configure.</param> 
    /// <returns>The (generic) host builder.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="InteractionService"/> is already added to the collection</exception>
    [Obsolete("This extension is obsolete and will be removed in a future version. Replace with builder.Services.AddInteractionService. See the Discord.Addons.Hosting repository for more information.")]
    public static IHostBuilder UseInteractionService(this IHostBuilder builder) => builder.ConfigureServices(
        (_, collection) =>
        {
            collection.AddInteractionService();
        }); 


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
    [Obsolete("This extension is obsolete and will be removed in a future version. Replace with builder.Services.AddInteractionService. See the Discord.Addons.Hosting repository for more information.")]
    public static IHostBuilder UseInteractionService(this IHostBuilder builder, Action<HostBuilderContext, InteractionServiceConfig> config)
    {
        ArgumentNullException.ThrowIfNull(config);

        builder.ConfigureServices((context, collection) =>
        {
            collection.AddInteractionService((interactionConfig, _) => config(context, interactionConfig));
        });

        return builder;
    }
}