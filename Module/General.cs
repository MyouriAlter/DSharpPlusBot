using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace DSharpPlusBot.Module
{
    public class General : BaseCommandModule
    {

        [Command("ping")]
        [Aliases("ms")]
        [Description("getting the ping")]
        public static async Task PingThisBotMore(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithAuthor(ctx.User.Username);
            embed.WithColor(DiscordColor.Aquamarine);
            embed.WithDescription($"ShardId for this client is : {ctx.Client.ShardId}");
            embed.WithTimestamp(DateTime.Now);
            embed.WithTitle($"Number of shards : {ctx.Client.ShardCount}");
            await ctx.Channel.SendMessageAsync(embed.Build());
        }

        [Command("res")]
        public async Task ResMes(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var message = await interactivity.WaitForMessageAsync
                (x => x.Channel == ctx.Channel)
                .ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(message.Result.Content);
        }
        
    }
}