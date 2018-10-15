# Discord.Addons.Hosting 
[![Build Status](https://dev.azure.com/GithubHawxy/Discord.Addons.Hosting/_apis/build/status/Hawxy.Discord.Addons.Hosting)](https://dev.azure.com/GithubHawxy/Discord.Addons.Hosting/_build/latest?definitionId=2)
[![NuGet](https://img.shields.io/nuget/v/Discord.Addons.Hosting.svg?style=flat-square)](https://www.nuget.org/packages/Discord.Addons.Hosting)

[Discord.Net](https://github.com/RogueException/Discord.Net) hosting with [Microsoft.Extensions.Hosting](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host). 
This package provides extensions to IHostBuilder that will run a Discord.Net socket/sharded client as a IHostedService. 

Discord.Net 2.0 build 1000 or later is required.

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

See [samples](https://github.com/Hawxy/Discord.Addons.Hosting/tree/master/Samples) for working examples
