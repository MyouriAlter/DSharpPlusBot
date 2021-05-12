using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlusBot.Utilities;

namespace DSharpPlusBot.Handler.Dialogue
{
    public abstract class DialogueStepBase : IDialogueStep
    {
        private readonly string _content;

        private readonly int _rgb = BotUtilities.RandomColor();

        protected DialogueStepBase(string content)
        {
            _content = content;
        }

        public Action<DiscordMessage> OnMessageAdded { get; set; }
            = delegate { };

        public abstract IDialogueStep NextStep { get; }

        public abstract Task<bool> ProcessStep(DiscordShardedClient shardedClient,
            DiscordChannel channel, DiscordUser user);

        protected async Task TryAgainThen(DiscordChannel channel, string problem)
        {
            var tryAgainEmbed = new DiscordEmbedBuilder
            {
                Title = "Please try again",
                Color = new DiscordColor(_rgb, _rgb, _rgb)
            };

            tryAgainEmbed.AddField("There was a problem with your previous input", problem);

            var embed = await channel.SendMessageAsync(tryAgainEmbed).ConfigureAwait(false);

            OnMessageAdded(embed);
        }
    }
}