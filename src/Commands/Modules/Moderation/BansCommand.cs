﻿using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules
{
    public partial class ModerationModule : VolteModule
    {
        [Command("Bans")]
        [Description("Shows all bans in this server.")]
        [Remarks("Usage: |prefix|bans")]
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task<VolteCommandResult> BansAsync()
        {
            var banList = await Context.Guild.GetBansAsync();
            if (!banList.Any())
            {
                return BadRequest("This server doesn't have anyone banned.");
            }

            return Ok(string.Join('\n',
                banList.Select(b => $"**{b.User}**: `{b.Reason ?? "No reason provided."}`")));
        }
    }
}