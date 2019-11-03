using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Attributes
{
    public sealed class RequireBotGuildPermissionAttribute : CheckAttribute
    {
        private readonly Permission[] _permissions;

        public RequireBotGuildPermissionAttribute(params Permission[] perms) => _permissions = perms;

        public override async ValueTask<CheckResult> CheckAsync(
            CommandContext context)
        {
            var ctx = context.Cast<VolteContext>();
            foreach (var perm in ctx.Guild.CurrentMember.Permissions.ToList())
            {
                if (ctx.Guild.CurrentMember.Permissions.Administrator)
                    return CheckResult.Successful;
                if (_permissions.Contains(perm))
                    return CheckResult.Successful;
            }

            await new LocalEmbedBuilder()
                .AddField("Error in Command", ctx.Command.Name)
                .AddField("Error Reason", $"I am missing the following guild-level permissions required to execute this command: `{ _permissions.Select(x => x.ToString()).Join(", ")}`")
                .AddField("Correct Usage", ctx.Command.GetUsage(ctx))
                .WithAuthor(ctx.User)
                .WithErrorColor()
                .SendToAsync(ctx.Channel);
            return CheckResult.Unsuccessful("Insufficient permission.");
        }
    }
}