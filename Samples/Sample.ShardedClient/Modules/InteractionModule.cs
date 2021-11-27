using Discord.Interactions;

namespace Sample.ShardedClient.Modules;

public class InteractionModule : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("echo", "Echo an input")]
    public async Task Echo(string input)
    {
        await RespondAsync(input);
    }
}