using Discord.WebSocket;
using System.Text.Json.Serialization;
using Disqord;
using Disqord.Bot;

namespace Volte.Core.Models.BotConfig
{
    public sealed class GuildLogging
    {
        internal GuildLogging()
        {
            Enabled = false;
            GuildId = ulong.MinValue;
            ChannelId = ulong.MinValue;
        }
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }


        public bool EnsureValidConfiguration(VolteBot bot, out CachedTextChannel channel)
        {
            if (!Enabled)
            {
                channel = null;
                return false;
            }

            if (GuildId is 0 || ChannelId is 0)
            {
                channel = null;
                return false;
            }

            var g = bot.GetGuild(GuildId);
            channel = g?.GetTextChannel(ChannelId);
            if (g is null)
            {
                return false;
            }

            return !(channel is null);
        }

    }
}