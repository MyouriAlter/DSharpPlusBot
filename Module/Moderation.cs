using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlusBot.Handler.Dialogue;

namespace DSharpPlusBot.Module
{
    public class Moderation : BaseCommandModule
    {
        private readonly DiscordShardedClient _shardedClient;

        public Moderation(DiscordShardedClient shardedClient)
        {
            _shardedClient = shardedClient;
        }
        
        
        [Command("crole")]
        [Description("Create a new role with no permission")]
        [Aliases("createrole", "newrole")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task CreateRole(CommandContext ctx, [RemainingText] string roleName)
        {
            await ctx.Guild.CreateRoleAsync($"{roleName}", Permissions.None, null, null, true);
            await ctx.RespondAsync($"Role : {roleName} has been created!").ConfigureAwait(false);
        }

        [Command("dialogue")]
        public async Task Dialogue(CommandContext ctx)
        {
            try
            {            
                var inputStep = new TextStep("Enter something here!", null);
                var mentionStep = new TextStep("Ha ha ha, nice try but I won't let you do that!", null, 10);

                var intStep = new IntStep("Ho ho ho", null, maxValue: 100);
                
                var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

                var input = string.Empty;

                int testNum = 0;

                inputStep.OnValidResult += (result) =>
                {
                    input = result;
                    
                    if (!result.Contains("@everyone") && (!result.Contains("@here"))) return;
                    
                    inputStep.SetNextStep(mentionStep);
                    userChannel.SendMessageAsync("Ha ha ha, nice try but I won't let you do that!");
                };

                intStep.OnValidResult += (result) => testNum = result;
                
                
                var inputDialogueHandler = new DialogueHandler(
                    _shardedClient,
                    userChannel,
                    ctx.User,
                    inputStep);

                var succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

                if (!succeeded)
                {
                    return;
                }

                await ctx.Channel.SendMessageAsync(input).ConfigureAwait(false);
                
                await ctx.Channel.SendMessageAsync(testNum.ToString()).ConfigureAwait(false);
                
            }
            catch (Exception e)
            {
                await ctx.RespondAsync("Your DM is blocked!");
            }

        }
        
    }
}