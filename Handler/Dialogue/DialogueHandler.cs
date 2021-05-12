using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlusBot.Utilities;

namespace DSharpPlusBot.Handler.Dialogue
{
    public class DialogueHandler
    {
        private readonly DiscordChannel _channel;

        private readonly List<DiscordMessage> _messages = new();

        private readonly int _rgb = BotUtilities.RandomColor();
        private readonly DiscordShardedClient _shardedClient;
        private readonly DiscordUser _user;
        private IDialogueStep _currentStep;

        public DialogueHandler(DiscordShardedClient shardedClient,
            DiscordChannel channel,
            DiscordUser user,
            IDialogueStep currentStep)
        {
            _shardedClient = shardedClient;
            _channel = channel;
            _user = user;
            _currentStep = currentStep;
        }

        public async Task<bool> ProcessDialogue()
        {
            while (_currentStep != null)
            {
                _currentStep.OnMessageAdded += message => _messages.Add(message);

                var cancelled = await _currentStep.ProcessStep(_shardedClient, _channel, _user).ConfigureAwait(false);

                if (cancelled)
                {
                    await DeleteMessage().ConfigureAwait(false);

                    var cancelEmbed = new DiscordEmbedBuilder
                    {
                        Title = "The dialogue has successfully been cancelled",
                        Description = _user.Mention,
                        Color = new DiscordColor(_rgb, _rgb, _rgb)
                    };

                    await _channel.SendMessageAsync(cancelEmbed).ConfigureAwait(false);

                    return false;
                }

                _currentStep = _currentStep.NextStep;
            }

            await DeleteMessage().ConfigureAwait(false);

            return true;
        }

        private async Task DeleteMessage()
        {
            if (_channel.IsPrivate) return;

            foreach (var message in _messages) await message.DeleteAsync().ConfigureAwait(false);
        }
    }
}