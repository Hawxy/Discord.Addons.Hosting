# Discord.Addons.Hosting 
[![Build Status](https://dev.azure.com/GithubHawxy/Discord.Addons.Hosting/_apis/build/status/Hawxy.Discord.Addons.Hosting)](https://dev.azure.com/GithubHawxy/Discord.Addons.Hosting/_build/latest?definitionId=2)
[![NuGet](https://img.shields.io/nuget/v/Discord.Addons.Hosting.svg?style=flat-square)](https://www.nuget.org/packages/Discord.Addons.Hosting)

[Discord.Net](https://github.com/RogueException/Discord.Net) hosting with [Microsoft.Extensions.Hosting](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host). 
This package provides extensions to a .NET Generic Host (IHostBuilder) that will run a Discord.Net socket/sharded client as a controllable IHostedService. This simplifies initial bot creation and moves the usual boilerplate to a convenient builder pattern.

Discord.Net 2.1.1+ & .NET Core 2.0+ is required.

```csharp
var builder = new HostBuilder()               
  .ConfigureAppConfiguration(x =>
  {
    //..configuration
  })
  .ConfigureLogging(x =>
  {
    //..logging
  })
  .ConfigureDiscordHost<DiscordSocketClient>((context, configurationBuilder) =>
  {
     configurationBuilder.SetDiscordConfiguration(new DiscordSocketConfig
     {
       LogLevel = LogSeverity.Verbose,
       AlwaysDownloadUsers = true,
       MessageCacheSize = 200
     });

    configurationBuilder.SetToken(context.Configuration["token"]);
  })
  //Omit this if you don't use the command service
  .UseCommandService()
  .ConfigureServices((context, services) =>
  {
    //Add any other services here
    services.AddHostedService<CommandHandler>();
  })
  .UseConsoleLifetime();

var host = builder.Build();
using (host)
{
  await host.RunAsync();
}
```

### Basic Usage

1. Create a .NET Core application (or retrofit your existing one)
2. Add the following NuGet packages (at the absolute minimum):

   ```Discord.Addons.Hosting```
   ```Microsoft.Extensions.Hosting```
   
3. Create and start your application using a HostBuilder as shown above and in the examples linked below

### Examples

Fully working examples are available [here](https://github.com/Hawxy/Discord.Addons.Hosting/tree/master/Samples)

If you want something more advanced, one of my bots CitizenEnforcer uses this extension. You can find it [here](https://github.com/Hawxy/CitizenEnforcer)

### Serilog

Serilog should be added to the host with ```Serilog.Extensions.Hosting```. 

See the Serilog [example](https://github.com/Hawxy/Discord.Addons.Hosting/tree/master/Samples/SampleBotSerilog) for usage

### Services

This section assumes some prior knowledge of Dependency Injection within the .NET ecosystem. Take a read of [this](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) and [this](https://discord.foxbot.me/stable/guides/commands/dependency-injection.html) if you have no idea what any of this means.

Most services people write within the Discord.NET world tend to fall in one of two buckets. One type gets instantiated & injected into `CommandModule`'s or other services by the container when required and simply exist to hold some basic state and/or abstract away common functionality. The other type is more complex and requires some kind of manual scaffolding to work as intended, such as subscribing to events published by the `DiscordSocketClient` or by performing an `async` request on creation. These services tend to operate in isolation and thus do not get initialized by being injected into a `CommandModule` or some other dependent service.

So, how do we initialize the latter type of service? Do we call `GetRequiredService<T>` and run an `Initialize()` method on every service that needs it? Do we create an attribute or interface and use reflection to get all the services we need to initialize? Do we just initialize them before adding them to the container? At a large scale, all of these solutions usually end up being a maintenance burden, an anti-pattern, or both.

Since we're using a `Host`, this problem is already solved, as the `IHostedService` can handle all of our initialization concerns for us. **Note: Implementations of `IHostedService` should not be injected into any other service/`CommandModule` etc, either separate your initialization concerns from your functional concerns or rethink your architecture.**

- I've included the base class `InitializedService` for services that simply need to be initialized once for the lifetime of the application (such as a `CommandHandler`, and any isolated service that just listens to client events). This base class implements `IHostedService` and simply keeps track of if `InitializeAsync` has been called already. 

```csharp
public class CommandHandler : InitializedService
{
    private readonly IServiceProvider _provider;
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commandService;
    private readonly IConfiguration _config;

    public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService commandService, IConfiguration config)
    {
        _provider = provider;
        _client = client;
        _commandService = commandService;
        _config = config;
    }
    public override async Task InitializeAsync(CancellationToken cancellationToken)
    {
        _client.MessageReceived += HandleMessage;
        _commandService.CommandExecuted += CommandExecutedAsync;
        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
    }
        
    //.....
}
 ````

- Services that run on a timer should either use the above pattern to start the timer, or an implementation of [BackgroundService](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice)

- Services with complex startup & shutdown activities should implement `IHostedService` directly.

### Shutdown

When shutdown is requested, the host will wait a maximum of 5 seconds for services to stop before timing out.

If you're finding that this isn't enough time, you can modify the shutdown timeout via the [ShutdownTimeout host setting](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0#shutdowntimeout).
