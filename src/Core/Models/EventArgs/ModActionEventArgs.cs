using System;
using Disqord;
using Volte.Commands;

namespace Volte.Core.Models.EventArgs
{
    public class ModActionEventArgs : System.EventArgs
    {
        public CachedMember Moderator { get; private set; }
        public VolteContext Context { get; private set; }
        public ModActionType ActionType { get; private set; }
        public string Reason { get; private set; }
        public ulong? TargetId { get; private set; }
        public CachedUser TargetUser { get; private set; }
        public int? Count { get; private set; }
        public DateTimeOffset Time { get; private set; }
        public CachedGuild Guild { get; private set; }

        public static ModActionEventArgs New => new ModActionEventArgs();

        public ModActionEventArgs WithContext(VolteContext ctx)
        {
            Context = ctx;
            return this;
        }

        public ModActionEventArgs WithModerator(CachedMember user)
        {
            Moderator = user;
            return this;
        }

        public ModActionEventArgs WithActionType(ModActionType type)
        {
            ActionType = type;
            return this;
        }

        public ModActionEventArgs WithReason(string reason)
        {
            Reason = reason;
            return this;
        }

        public ModActionEventArgs WithTarget(ulong? id)
        {
            TargetId = id;
            return this;
        }

        public ModActionEventArgs WithTarget(CachedUser user)
        {
            TargetUser = user;
            return this;
        }

        public ModActionEventArgs WithCount(int? count)
        {
            Count = count;
            return this;
        }

        public ModActionEventArgs WithTime(DateTimeOffset time)
        {
            Time = time;
            return this;
        }

        public ModActionEventArgs WithGuild(CachedGuild guild)
        {
            Guild = guild;
            return this;
        }

        public ModActionEventArgs WithDefaultsFromContext(VolteContext ctx)
        {
            WithContext(ctx)
                .WithTime(ctx.Now)
                .WithGuild(ctx.Guild)
                .WithModerator(ctx.Member);


            return this;
        }
    }
}