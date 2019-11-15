﻿using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("DeleteMessageOnCommand", "Dmoc")]
        [Description("Enable/Disable deleting the command message upon execution of a command for this guild.")]
        [Remarks("deletemessageoncommand {true|false}")]
        [RequireGuildAdmin]
        public Task<ActionResult> DeleteMessageOnCommandAsync(bool enabled)
        {
            Db.ModifyData(Context.Guild, data => data.Configuration.DeleteMessageOnCommand = enabled);
            return Ok(enabled
                ? "Enabled DeleteMessageOnCommand in this guild."
                : "Disabled DeleteMessageOnCommand in this guild.");
        }

        [Command("DeleteMessageOnTagCommand", "Dmotc")]
        [Description(
            "Enable/Disable deleting the command message upon usage of the tag retrieval command for this guild.")]
        [Remarks("deletemessageontagcommand {true|false}")]
        public Task<ActionResult> DeleteMessageOnTagCommand(bool enabled)
        {
            Db.ModifyData(Context.Guild, data => data.Configuration.DeleteMessageOnTagCommandInvocation = enabled);
            return Ok(enabled
                ? "Enabled DeleteMessageOnTagCommand in this guild."
                : "Disabled DeleteMessageOnTagCommand in this guild.");
        }
    }
}