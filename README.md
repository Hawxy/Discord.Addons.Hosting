# Discord.Addons.Hosting 
[![NuGet](https://img.shields.io/nuget/v/Discord.Addons.Hosting.svg?style=flat-square)](https://www.nuget.org/packages/Discord.Addons.Hosting)
![Nuget](https://img.shields.io/nuget/dt/Discord.Addons.Hosting?style=flat-square)

[Discord.NET](https://github.com/RogueException/Discord.Net) hosting with [Microsoft.Extensions.Hosting](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host). 
This package provides extensions that will run a Discord.NET socket/sharded client as an `IHostedService`, featuring:

✅ Simplified, best practice bot creation with a reduction in boilerplate.

✅ Instant wire-up of Logging and Dependency Injection support.

✅ Extensions to easily run startup & background tasks involving the Discord Client.

✅ Easy integration with other generic host consumers, such as ASP.NET Core.

.NET  6.0+ is required.

```csharp
// CreateApplicationBuilder configures a lot of stuff for us automatically
// See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host
var builder = Host.CreateApplicationBuilder(args);

// Configure Discord.NET
builder.Services.AddDiscordHost((config, _) =>
{
    config.SocketConfig = new DiscordSocketConfig
    {
        LogLevel = LogSeverity.Verbose,
        AlwaysDownloadUsers = true,
        MessageCacheSize = 200,
        GatewayIntents = GatewayIntents.All
    };

    config.Token = builder.Configuration["Token"]!;
});

// Optionally wire up the command service
builder.Services.AddCommandService((config, _) =>
{
    config.DefaultRunMode = RunMode.Async;
    config.CaseSensitiveCommands = false;
});

// Optionally wire up the interaction service
builder.Services.AddInteractionService((config, _) =>
{
    config.LogLevel = LogSeverity.Info;
    config.UseCompiledLambda = true;
});
// Add any other services here
builder.Services.AddHostedService<CommandHandler>();
builder.Services.AddHostedService<InteractionHandler>();
builder.Services.AddHostedService<BotStatusService>();
builder.Services.AddHostedService<LongRunningService>();

var host = builder.Build();

await host.RunAsync();
```

## Getting Started

1. Create a [.NET 8 Worker Service](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio#worker-service-template) using Visual Studio or via the dotnet cli (`dotnet new worker -o MyWorkerService`)
2. Add `Discord.Addons.Hosting` to your project.   
3. Set your bot token via the [dotnet secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#set-a-secret): `dotnet user-secrets set "token" "your-token-here"`
4. Add your bot prefix to `appsettings.json`
5. Configure your Discord client with `ConfigureDiscordHost`.
6. Enable the `CommandService` and/or the `InteractionService` with `UseCommandService` and `UseInteractionService`
7. Create and start your application using a HostBuilder as shown above and in the examples linked below.

## Examples

Fully working examples are available [here](https://github.com/Hawxy/Discord.Addons.Hosting/tree/master/Samples)

## Sharded Client

To use the sharded client instead of the socket client, simply replace `ConfigureDiscordHost` with `ConfigureDiscordShardedHost`:
```csharp
.AddDiscordShardedHost((config, _) =>
{
    config.SocketConfig = new DiscordSocketConfig
    {
    	// Manually set the required shards, or leave empty for the recommended count
	    TotalShards = 4
    };

    config.Token = context.Configuration["token"];
})

```

## Serilog

Microsoft's default logging has an unfortunate default output format, so I highly recommend using Serilog instead of the standard Microsoft logging. 

Serilog should be added to the host with ```Serilog.Extensions.Hosting```. 

See the Serilog [example](https://github.com/Hawxy/Discord.Addons.Hosting/tree/master/Samples/SampleBotSerilog) for usage.

## Discord Client Services

This section assumes some prior knowledge of Dependency Injection within the .NET ecosystem. Take a read of [this](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) if you have no idea what any of this means.

During bot development, it's highly like you'll require the ability to execute code immediately after startup, such as setting the bot's status, reaching out to a web server, registering an event, or kicking off the continuous execution of code in the background. Given we're using the generic host and have its `IHostedService` & `BackgroundService` capabilities in our toolbelt, this is easily achievable in a clean and concise way. 

This package ships with the `DiscordClientService` and `DiscordShardedClientService` base classes for the socket client and sharded client respectively. For convenience, both of them expose the `Client` and `Logger`. Simply inherit from the given type, implement the required constructor, place your execution requirements within `ExecuteAsync` and register the service with your service collection via `services.AddHostedService`.

```csharp
public class CommandHandler : DiscordClientService
{
    private readonly IServiceProvider _provider;
    private readonly CommandService _commandService;
    private readonly IConfiguration _config;

    public CommandHandler(DiscordSocketClient client, ILogger<CommandHandler> logger,  IServiceProvider provider, CommandService commandService, IConfiguration config) : base(client, logger)
    {
        _provider = provider;
        _commandService = commandService;
        _config = config;
    }
    // This'll be executed during startup.
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Client.MessageReceived += HandleMessage;
        _commandService.CommandExecuted += CommandExecutedAsync;
        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
    }
        
    //.....
}
```
```csharp
 builder.Services.AddHostedService<CommandHandler>();
```
 
 The `WaitForReadyAsync` extension method is also available for both client types to await execution of your service until the client has reached a Ready state:
 
 ```csharp
public class BotStatusService : DiscordClientService
{
    public BotStatusService(DiscordSocketClient client, ILogger<DiscordClientService> logger) : base(client, logger)
    {
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
    	// Wait for the client to be ready before setting the status
        await Client.WaitForReadyAsync(stoppingToken);
        Logger.LogInformation("Client is ready!");

        await Client.SetActivityAsync(new Game("Set my status!"));
    }
}
```

#### Additional notes: 
- Services that do not require access to the Discord Client should use an implementation of [BackgroundService](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice).

- Services with complex startup & shutdown activities should implement `IHostedService` directly.

### Shutdown

When shutdown is requested, the host will wait a maximum of 5 seconds for services to stop before timing out.

If you're finding that this isn't enough time, you can modify the shutdown timeout via the [ShutdownTimeout host setting](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-8.0#shutdowntimeout).

### IOptions

This package uses `Microsoft.Extensions.Options` internally, so both the `DiscordHostConfiguration` and `CommandServiceConfig` can be configured within the services registration instead of within the `HostBuilder` extensions if it better suits your scenario.
