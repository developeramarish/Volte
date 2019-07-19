﻿using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models.Results;

namespace Volte.Commands.Modules.AdminUtility
{
    public partial class AdminUtilityModule : VolteModule
    {
        [Command("ServerName", "Sn")]
        [Description("Sets the name of the current server.")]
        [Remarks("Usage: |prefix|servername {name}")]
        [RequireBotGuildPermission(GuildPermission.ManageGuild | GuildPermission.Administrator)]
        [RequireGuildAdmin]
        public async Task<BaseResult> ServerNameAsync([Remainder] string name)
        {
            await Context.Guild.ModifyAsync(g => g.Name = name);
            return Ok($"Set this server's name to **{name}**!");
        }
    }
}