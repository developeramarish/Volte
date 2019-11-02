using System.Collections.Generic;
using System.Text.Json.Serialization;
using Disqord;
using Gommon;

namespace Volte.Core.Models.Guild
{
    public sealed class ModerationOptions
    {
        internal ModerationOptions()
            => Blacklist = new List<string>();

        [JsonPropertyName("mass_ping_checks")]
        public bool MassPingChecks { get; set; }

        [JsonPropertyName("antilink")]
        public bool Antilink { get; set; }

        [JsonPropertyName("mod_log_channel")]
        public ulong ModActionLogChannel { get; set; }

        [JsonPropertyName("mod_role")]
        public ulong ModRole { get; set; }

        [JsonPropertyName("admin_role")]
        public ulong AdminRole { get; set; }

        [JsonPropertyName("blacklist")]
        public List<string> Blacklist { get; set; }
    }

    public sealed class WelcomeOptions
    {

        [JsonPropertyName("welcome_channel")]
        public ulong WelcomeChannel { get; set; }

        [JsonPropertyName("welcome_message")]
        public string WelcomeMessage { get; set; }

        [JsonPropertyName("leaving_message")]
        public string LeavingMessage { get; set; }

        [JsonPropertyName("welcome_color")]
        public int WelcomeColor { get; set; }

        [JsonPropertyName("welcome_dm_message")]
        public string WelcomeDmMessage { get; set; }

        public string FormatWelcomeMessage(CachedMember user) 
            => WelcomeMessage.ReplaceIgnoreCase("{ServerName}", user.Guild.Name)
                .ReplaceIgnoreCase("{GuildName}", user.Guild.Name)
                .ReplaceIgnoreCase("{UserName}", user.Name)
                .ReplaceIgnoreCase("{UserMention}", user.Mention)
                .ReplaceIgnoreCase("{OwnerMention}", user.Guild.Owner.Mention)
                .ReplaceIgnoreCase("{UserTag}", user.Discriminator)
                .ReplaceIgnoreCase("{MemberCount}", user.Guild.MemberCount)
                .ReplaceIgnoreCase("{UserString}", user);

        public string FormatLeavingMessage(CachedUser user, CachedGuild guild) 
            => LeavingMessage.ReplaceIgnoreCase("{ServerName}", guild.Name)
                .ReplaceIgnoreCase("{GuildName}", guild.Name)
                .ReplaceIgnoreCase("{UserName}", user.Name)
                .ReplaceIgnoreCase("{UserMention}", user.Mention)
                .ReplaceIgnoreCase("{OwnerMention}", guild.Owner.Mention)
                .ReplaceIgnoreCase("{UserTag}", user.Discriminator)
                .ReplaceIgnoreCase("{MemberCount}", guild.MemberCount)
                .ReplaceIgnoreCase("{UserString}", user);

        public string FormatDmMessage(CachedMember user)
            => WelcomeDmMessage.ReplaceIgnoreCase("{ServerName}", user.Guild.Name)
                .ReplaceIgnoreCase("{GuildName}", user.Guild.Name)
                .ReplaceIgnoreCase("{UserName}", user.Name)
                .ReplaceIgnoreCase("{UserMention}", user.Mention)
                .ReplaceIgnoreCase("{OwnerMention}", user.Guild.Owner.Mention)
                .ReplaceIgnoreCase("{UserTag}", user.Discriminator)
                .ReplaceIgnoreCase("{MemberCount}", user.Guild.MemberCount)
                .ReplaceIgnoreCase("{UserString}", user);
    }
}