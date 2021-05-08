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
        private DiscordShardedClient _shardedClient;

        private IReadOnlyDictionary<int, CommandsNextExtension> _commands;

        public InteractivityExtension _interactivityExtension;

        private CancellationTokenSource _cancellationTokenSource;


        public async Task RunClientAsync()
        {

            #region json.config
            string json;

            await using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            
            #endregion

            this._cancellationTokenSource = new CancellationTokenSource();

            #region Initializations
            
            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                AlwaysCacheMembers = true,
                MessageCacheSize = 1337,
            };

            this._shardedClient = 
               new DiscordShardedClient(
               new DiscordConfiguration(config));

            #endregion
            
            #region interactive register

            await this._shardedClient.UseInteractivityAsync(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2),
                PaginationDeletion = PaginationDeletion.DeleteEmojis,
                PaginationBehaviour = PaginationBehaviour.WrapAround
            }).ConfigureAwait(false);

            #endregion
           
            #region commandConfig

            var commandConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] {configJson.Prefix},
                CaseSensitive = false,
                DmHelp = true,
                EnableDms = true,
                EnableDefaultHelp = true,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true,
                Services = new ServiceCollection()
                    .AddSingleton(this)
                    .BuildServiceProvider()
            };
            
            _commands = await this._shardedClient.UseCommandsNextAsync(commandConfig);
            #endregion

            #region command register module

            foreach (var cmd in _commands.Values)
            {
                cmd.RegisterCommands<General>();
                cmd.RegisterCommands<Moderation>();
            }

            #endregion

            
            #region start up the bot
            
            await this._shardedClient.StartAsync();

            await Task.Delay(-1);

            #endregion
            
            #region Client event

            _shardedClient.Ready += ShardedClientOnReady;

            #endregion

           
            #region shut down

            while (!this._cancellationTokenSource.IsCancellationRequested) 
                await Task.Delay(2000);
            await this._shardedClient.StopAsync();

            #endregion

        }

        private static Task ShardedClientOnReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}