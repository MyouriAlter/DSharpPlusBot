using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlusBot.Module;
using DSharpPlusBot.Struct;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DSharpPlusBot.Client
{
    public class OliviaClient
    {
        private CancellationTokenSource _cancellationTokenSource;
        private DiscordShardedClient ShardedClient { get; set; }

        private IReadOnlyDictionary<int, CommandsNextExtension> Commands { get; set; }

        private IReadOnlyDictionary<int, InteractivityExtension> InteractivityExtension { get; set; }

        public async Task RunClientAsync()
        {
            #region json.config

            string json;

            await using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            {
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            }

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            #endregion

            _cancellationTokenSource = new CancellationTokenSource();

            #region Initializations

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                AlwaysCacheMembers = true,
                MessageCacheSize = 1337
            };

            ShardedClient =
                new DiscordShardedClient(
                    new DiscordConfiguration(config));

            #endregion

            #region interactive register

            var interactiveConfig = new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2),
                PaginationDeletion = PaginationDeletion.DeleteEmojis,
                PaginationBehaviour = PaginationBehaviour.WrapAround
            };

            InteractivityExtension = await ShardedClient.UseInteractivityAsync(interactiveConfig);

            //this._interactivityExtension = await this.ShardedClient.UseInteractivityAsync(interactiveConfig);

            #endregion

            #region commandConfig

            var service = new ServiceCollection();

            var commandConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new[] {configJson.Prefix},
                CaseSensitive = false,
                DmHelp = true,
                EnableDms = true,
                EnableDefaultHelp = true,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true,
                Services = service
                    .AddSingleton(ShardedClient)
                    .BuildServiceProvider()
            };

            Commands = await ShardedClient.UseCommandsNextAsync(commandConfig);

            #endregion

            #region command register module

            foreach (var cmd in Commands.Values)
            {
                cmd.RegisterCommands<General>();
                cmd.RegisterCommands<Moderation>();
            }

            #endregion

            #region start up the bot

            await ShardedClient.StartAsync();

            await Task.Delay(-1);

            #endregion

            #region Client event

            ShardedClient.Ready += ShardedClientOnReady;

            #endregion


            #region shut down

            while (!_cancellationTokenSource.IsCancellationRequested)
                await Task.Delay(2000);
            await ShardedClient.StopAsync();

            #endregion
        }

        private static Task ShardedClientOnReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}