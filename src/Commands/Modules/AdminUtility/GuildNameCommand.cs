using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule : VolteModule
    {
        [Command("GuildName", "Gn", "ServerName", "Sn")]
        [Description("Sets the name of the current guild.")]
        [Remarks("servername {name}")]
        [RequireBotGuildPermission(Permission.ManageGuild)]
        [RequireGuildAdmin]
        public async Task<ActionResult> ServerNameAsync([Remainder] string name)
        {
            await Context.Guild.ModifyAsync(g => g.Name = name);
            return Ok($"Set this guild's name to **{name}**!");
        }
    }
}