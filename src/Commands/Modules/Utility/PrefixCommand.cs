﻿using System.Threading.Tasks;
using Discord;
 
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Prefix")]
        [Description("Shows the command prefix for this guild.")]
        [Remarks("Usage: |prefix|prefix")]
        public Task<ActionResult> PrefixAsync()
        {
            return Ok(
                $"The prefix for this guild is **{Context.GuildData.Configuration.CommandPrefix}**; alternatively you can just mention me as a prefix, i.e. `@{Context.Client.CurrentUser.Username} help`.");
        }
    }
}