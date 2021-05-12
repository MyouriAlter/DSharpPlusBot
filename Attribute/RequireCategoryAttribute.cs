using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DSharpPlusBot.Attribute
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequireCategoryAttribute : CheckBaseAttribute
    {
        public RequireCategoryAttribute(ChannelCheckMode checkMode, params string[] categoryNames)
        {
            CheckMode = checkMode;
            CategoryNames = new ReadOnlyCollection<string>(categoryNames);
        }

        private IReadOnlyList<string> CategoryNames { get; }
        private ChannelCheckMode CheckMode { get; }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if (ctx.Guild == null || ctx.Member == null) return Task.FromResult(false);

            var contains = CategoryNames.Contains(ctx.Channel.Parent.Name,
                StringComparer.OrdinalIgnoreCase);

            return CheckMode switch
            {
                ChannelCheckMode.Any => Task.FromResult(contains),

                ChannelCheckMode.None => Task.FromResult(!contains),

                _ => Task.FromResult(false)
            };
        }
    }
}