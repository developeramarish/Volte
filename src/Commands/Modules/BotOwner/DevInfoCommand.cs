using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;
using Volte.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("DevInfo", "Di")]
        [Description("Shows information about the bot and about the system it's hosted on.")]
        [Remarks("devinfo")]
        [RequireBotOwner]
        public Task<ActionResult> DevInfoAsync() 
            => Ok(FormatHelper.Code(new StringBuilder()
                    .AppendLine("== Core ==")
                    .AppendLine($"{Context.Bot.Guilds.Count} Guilds")
                    .AppendLine($"{Context.Bot.Guilds.Values.Sum(x => x.Channels.Count)} Text/Voice Channels")
                    .AppendLine("== Commands ==")
                    .AppendLine($"{CommandService.GetAllModules().Count} Modules")
                    .AppendLine($"{CommandService.GetAllCommands().Count} Commands")
                    .AppendLine($"{CommandService.GetTotalTypeParsers()} TypeParsers")
                    .AppendLine("== Environment ==")
                    .AppendLine($"OS: {Environment.OSVersion}")
                    .AppendLine($"Processor Count: {Environment.ProcessorCount}")
                    .AppendLine($"Is 64-bit OS: {Environment.Is64BitOperatingSystem}")
                    .AppendLine($"Is 64-bit Process: {Environment.Is64BitProcess}")
                    .AppendLine($"Current Thread ID: {Environment.CurrentManagedThreadId}")
                    .AppendLine($"System Name: {Environment.MachineName}")
                    .AppendLine($".NET Core Version: {Environment.Version}")
                    .AppendLine($"Culture: {CultureInfo.InstalledUICulture.EnglishName}")
                    .AppendLine($"System Directory: {Environment.SystemDirectory}")
                    .ToString(), "json"));

    }
}
