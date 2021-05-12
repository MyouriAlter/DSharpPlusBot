using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlusBot.Utilities;

namespace DSharpPlusBot.Handler.Dialogue
{
    public class IntStep : DialogueStepBase
    {
        private readonly int? _minValue;
        private readonly int? _maxValue;
        private IDialogueStep _nextStep;
        
        private readonly int _rgb = BotUtilities.RandomColor();

        public IntStep(string content, IDialogueStep nextStep,
            int? minvalue = null, int? maxValue = null) : base(content)
        {
            NextStep = nextStep;
            _minValue = minvalue;
            _maxValue = maxValue;
        }

        public Action<int> OnValidResult { get; set; } = delegate(int s) {  };

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

            if (_maxValue.HasValue)
            {
                responseEmbed.AddField("Max value: ", $"{_maxValue} ");
            }

            if (_minValue.HasValue)
            {
                responseEmbed.AddField("Min value: ", $"{_minValue} ");
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

                    if (!int.TryParse(messageResult.Result.Content, out var inputValue))
                    {
                        await TryAgainThen(channel, $"Your input is not an integer!!").ConfigureAwait(false);
                        continue;
                    }

                    if (inputValue < _minValue)
                    {
                        await TryAgainThen(channel,
                            $"You input value is : {inputValue} is smaller than the {_minValue}").ConfigureAwait(false);
                        continue;
                    }

                    if (inputValue > _maxValue)
                    {
                        await TryAgainThen(channel,
                            $"You input value is : {inputValue} is greater than the {_maxValue}").ConfigureAwait(false);
                        continue;
                    }

                    OnValidResult(inputValue);

                    return false;
                }
            }
            return false;
        }
    }
}