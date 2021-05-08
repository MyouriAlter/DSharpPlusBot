using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace DSharpPlusBot.Module
{
    public class Moderation : BaseCommandModule
    {
        [Command("join")]
        public async Task Join(CommandContext ctx)
        {
            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "Would you like to join?"
            };

            await ctx.Channel.SendMessageAsync(joinEmbed.Build()).ConfigureAwait(false);
        }
    }
}