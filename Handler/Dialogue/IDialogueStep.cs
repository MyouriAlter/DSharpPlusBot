using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace DSharpPlusBot.Handler.Dialogue
{
    public interface IDialogueStep
    {
        Action<DiscordMessage> OnMessageAdded { get; set; }

        IDialogueStep NextStep { get; }

        Task<bool> ProcessStep(DiscordShardedClient shardedClient,
            DiscordChannel channel, DiscordUser user);
    }
}