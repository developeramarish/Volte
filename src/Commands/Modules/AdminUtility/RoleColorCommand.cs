﻿using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules.AdminUtility
{
    public partial class AdminUtilityModule : VolteModule
    {
        [Command("RoleColor", "RoleClr", "Rcl")]
        [Description("Changes the color of a specified role.")]
        [Remarks("Usage: |prefix|rolecolor {role} {r} {g} {b}")]
        [RequireBotGuildPermission(GuildPermission.ManageRoles | GuildPermission.Administrator)]
        [RequireGuildAdmin]
        public async Task<BaseResult> RoleColorAsync(SocketRole role, int r, int g, int b)
        {
            await role.ModifyAsync(x => x.Color = new Color(r, g, b));
            return Ok($"Successfully changed the color of the role **{role.Name}**.");
        }
    }
}