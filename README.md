# Discord.Addons.Hosting 
[![Build Status](https://dev.azure.com/GithubHawxy/Discord.Addons.Hosting/_apis/build/status/Hawxy.Discord.Addons.Hosting)](https://dev.azure.com/GithubHawxy/Discord.Addons.Hosting/_build/latest?definitionId=2)
[![NuGet](https://img.shields.io/nuget/v/Discord.Addons.Hosting.svg?style=flat-square)](https://www.nuget.org/packages/Discord.Addons.Hosting)

[Discord.Net](https://github.com/RogueException/Discord.Net) hosting with [Microsoft.Extensions.Hosting](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host). 
This package primarily provides extensions to a .NET Generic Host (IHostBuilder) that will run a Discord.Net socket/sharded client as a controllable IHostedService. This simplifies initial bot creation and moves the usual boilerplate to a convenient builder pattern.

Discord.Net 2.0.1 or later is required.

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
  .ConfigureDiscordClient<DiscordSocketClient>((context, discordBuilder) =>
  {
    var config = new DiscordSocketConfig();
    discordBuilder.UseDiscordConfiguration(config);
  })
  //Omit this if you don't use the command service
  .UseCommandService()
  .ConfigureServices((context, services) =>
  {
    //Add any other services here
    services.AddSingleton<CommandHandler>();
  })
  .UseConsoleLifetime();

var host = builder.Build();
using (host)
{
  await host.Services.GetRequiredService<CommandHandler>().InitializeAsync();
  await host.RunAsync();
}
```

### Basic Usage

1. Create a .NET Core application (or retrofit your existing one)
2. Add the following NuGet packages (at the absolute minimum):

   ```Discord.Addons.Hosting```
   ```Microsoft.Extensions.Hosting```
   ```Microsoft.Extensions.Configuration.Json```
   
3. Create and start your application using a HostBuilder as shown above and in the examples linked below

### Examples

Fully working examples are available [here](https://github.com/Hawxy/Discord.Addons.Hosting/tree/master/Samples)

If you want something more advanced, one of my bots CitizenEnforcer uses this extension. You can find it [here](https://github.com/Hawxy/CitizenEnforcer)

### Serilog

Serilog should be added to the host with ```Serilog.Extensions.Hosting```. 

See the Serilog [example](https://github.com/Hawxy/Discord.Addons.Hosting/tree/master/Samples/SampleBotSerilog) for usage

### Shutdown

When shutdown is requested, the host will wait a maximum of 5 seconds for services to stop before timing out.

If you're finding that this isn't enough time, you can modify the shutdown timeout via the [relevant host setting](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.2#shutdown-timeout).

### Reliability 

Discord.Net can occasionally fail to reconnect after an extended outage. This library provides a basic solution that will automatically attempt to restart the host on a failure. Please note that this functionality is experimental and does not guarantee that the client will *always* recover. Please note that this feature is also affected by the shutdown timeout set above.

To use the reliability extensions, start the host with ```await host.RunReliablyAsync()```.

To shutdown the host, it's recommended to add a shutdown command to your bot and call ```host.StopReliablyAsync()```.

This behaviour is similar to the usage of ```RunAsync()``` and ```StopAsync()```
