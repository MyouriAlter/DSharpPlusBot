using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlusBot.Utilities;

namespace DSharpPlusBot.Handler.Dialogue
{
    public class TextStep : DialogueStepBase
    {
        private readonly int? _minLength;
        private readonly int? _maxLength;
        private IDialogueStep _nextStep;
        
        private readonly int _rgb = BotUtilities.RandomColor();

        public TextStep(string content, IDialogueStep nextStep,
            int? minLength = null, int? maxLength = null) : base(content)
        {
            NextStep = nextStep;
            _minLength = minLength;
            _maxLength = maxLength;
        }
        
        public Action<string> OnValidResult { get; set; } = delegate(string s) {  };

        public override IDialogueStep NextStep { get; }

        public void SetNextStep(IDialogueStep nextStep)
        {
            _nextStep = nextStep;
        }

        public override async Task<bool> ProcessStep(DiscordShardedClient shardedClient, DiscordChannel channel,
            DiscordUser user)
        {
            var responseEmbed = new DiscordEmbedBuilder
            {
                Title = "Please response below",
                Color = new DiscordColor(_rgb, _rgb, _rgb)
            };

            responseEmbed.AddField("To stop the dialogue", "use the **?cancel** command");

            if (_maxLength.HasValue)
            {
                responseEmbed.AddField("Max length: ", $"{_maxLength} characters");
            }

            if (_minLength.HasValue)
            {
                responseEmbed.AddField("Min length: ", $"{_minLength} characters");
            }

            foreach (var interactivityClient in shardedClient.ShardClients)
            {

                var interactivity = interactivityClient.Value.GetInteractivity();

                while (true)
                {
                    var embed = await channel.SendMessageAsync(embed: responseEmbed).ConfigureAwait(false);

                    OnMessageAdded(embed);

                    var messageResult = await interactivity.WaitForMessageAsync(
                        x => x.Channel.Id == channel.Id && x.Author.Id == user.Id).ConfigureAwait(false);

                    OnMessageAdded(messageResult.Result);

                    if (messageResult.Result.Content.Equals("?cancel", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }

                    if (_minLength.HasValue)
                    {
                        if (messageResult.Result.Content.Length < _minLength.Value)
                        {
                            await TryAgainThen(channel,
                                $"You input is {_minLength.Value - messageResult.Result.Content.Length} characters short");
                            continue;
                        }
                    }

                    if (_maxLength.HasValue)
                    {
                        if (messageResult.Result.Content.Length > _maxLength.Value)
                        {
                            await TryAgainThen(channel,
                                $"You input is {messageResult.Result.Content.Length - _maxLength.Value} characters long");
                            continue;
                        }
                    }

                    OnValidResult(messageResult.Result.Content);

                    return false;
                }
            }
            return false;
        }
    }
}