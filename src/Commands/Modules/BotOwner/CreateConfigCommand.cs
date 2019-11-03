using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("CreateConfig")]
        [Description("Create a config for the guild with the given ID, if one doesn't exist.")]
        [Remarks("createconfig [serverId]")]
        [RequireBotOwner]
        public Task<ActionResult> CreateConfigAsync(ulong guildId = 0)
            => Ok($"Created a config for **{Context.Bot.GetGuild(guildId).Name}** if it didn't exist.", m =>
            {
                _ = Db.GetData(guildId is 0 ? Context.Guild.Id.RawValue : guildId);
                return Task.CompletedTask;
            });
    }
}