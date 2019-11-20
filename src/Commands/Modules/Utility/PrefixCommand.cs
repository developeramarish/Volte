﻿using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Prefix")]
        [Description("Shows the command prefix for this guild.")]
        [Remarks("prefix")]
        public Task<ActionResult> PrefixAsync() 
            => Ok($"The prefix for this guild is **{Context.GuildData.Configuration.CommandPrefix}**; alternatively you can just mention me as a prefix, i.e. `@{Context.Bot.CurrentUser.Name} help`.");
    }
}