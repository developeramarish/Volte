using System;
using System.Threading.Tasks;
using Disqord;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CheckHierarchyAttribute : ParameterCheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(object argument, CommandContext context)
        {
            var u = argument.Cast<CachedMember>() ?? throw new ArgumentException($"Cannot use the CheckHierarchy attribute on a type that isn't {typeof(CachedMember)}.");
            var ctx = context.Cast<VolteContext>();

            return ctx.Member.Hierarchy >= u.Hierarchy
                ? CheckResult.Successful
                : CheckResult.Unsuccessful("Cannot ban someone in a higher, or equal, hierarchy position than yourself.");
        }
    }
}