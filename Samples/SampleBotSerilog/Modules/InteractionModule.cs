using System.Threading.Tasks;
using Discord.Interactions;

namespace Sample.Serilog.Modules
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("echo", "Echo an input")]
        public async Task Echo(string input)
        {
            await RespondAsync(input);
        }
    }
}
