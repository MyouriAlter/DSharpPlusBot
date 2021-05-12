using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlusBot.Attribute;

namespace DSharpPlusBot.Module
{
    public class General : BaseCommandModule
    {
        private readonly DiscordShardedClient _shardedClient;

        public General(DiscordShardedClient shardedClient)
        {
            _shardedClient = shardedClient;
        }

        [Command("ping")]
        [Aliases("ms")]
        [Description("getting the ping")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [RequireCategory(ChannelCheckMode.Any, "abc", "bcd")]
        public async Task PingThisBotMore(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithAuthor(ctx.User.Username);
            embed.WithColor(DiscordColor.Aquamarine);
            embed.WithDescription($"ShardId for this client is : {_shardedClient.GetShard(ctx.Guild.Id).ShardId}");
            embed.WithTimestamp(DateTime.Now);
            embed.WithTitle($"Number of shards : {_shardedClient.ShardClients.Count} " +
                            "&& " +
                            $"latency : {_shardedClient.GetShard(ctx.Guild.Id).Ping} ms");
            await ctx.Channel.SendMessageAsync(embed.Build()).ConfigureAwait(false);
        }

        [Command("res")]
        public async Task ResMes(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var message = await interactivity.WaitForMessageAsync
                    (x => x.Channel == ctx.Channel)
                .ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(message.Result.Content).ConfigureAwait(false);
        }

        [Command("poll")]
        public async Task Poll(CommandContext ctx, TimeSpan duration, params DiscordEmoji[] option)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var options = option.Select(x => x.ToString());

            var embed = new DiscordEmbedBuilder();
        }
    }
}