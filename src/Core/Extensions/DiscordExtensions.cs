using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Disqord;
using Disqord.Rest;
using Volte.Core;
using Volte.Core.Models.EventArgs;
using Volte.Services;

namespace Gommon
{
    public static partial class Extensions
    {
        public static bool IsBotOwner(this CachedMember user)
            => Config.Owner == user.Id;

        private static bool IsGuildOwner(this CachedMember user)
            => user.Guild.OwnerId == user.Id || IsBotOwner(user);

        public static bool IsModerator(this CachedMember user, IServiceProvider provider)
        {
            provider.Get<DatabaseService>(out var db);
            return HasRole(user, db.GetData(user.Guild.Id).Configuration.Moderation.ModRole) ||
                   IsAdmin(user, provider) ||
                   IsGuildOwner(user);
        }

        private static bool HasRole(this CachedMember user, ulong roleId)
            => user.Roles.Select(x => x.Value.Id).Contains(new Snowflake(roleId));

        public static bool IsAdmin(this CachedMember user, IServiceProvider provider)
        {
            provider.Get<DatabaseService>(out var db);
            return HasRole(user,
                       db.GetData(user.Guild.Id).Configuration.Moderation.AdminRole) ||
                   IsGuildOwner(user);
        }

        public static async Task<bool> TrySendMessageAsync(this CachedMember user, string text = null,
            bool isTts = false, LocalEmbed embed = null, RestRequestOptions options = null)
        {
            try
            {
                await user.SendMessageAsync(text, isTts, embed, options);
                return true;
            }
            catch (DiscordHttpException e) when (e.HttpStatusCode is HttpStatusCode.Forbidden)
            {
                return false;
            }
        }

        public static async Task<bool> TrySendMessageAsync(this CachedTextChannel channel, string text = null,
            bool isTts = false, LocalEmbed embed = null, RestRequestOptions options = null)
        {
            try
            {
                await channel.SendMessageAsync(text, isTts, embed, options);
                return true;
            }
            catch (DiscordHttpException e) when (e.HttpStatusCode is HttpStatusCode.Forbidden)
            {
                return false;
            }
        }

        public static string GetInviteUrl(this DiscordClientBase client, bool withAdmin = true)
            => withAdmin
                ? $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=8"
                : $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=402992246";

        public static CachedUser GetOwner(this DiscordClientBase client)
            => client.GetUser(Config.Owner);

        public static CachedGuild GetPrimaryGuild(this DiscordClientBase client)
            => client.GetGuild(405806471578648588);

        public static Task RegisterVolteEventHandlersAsync(this VolteBot bot)
        {
            bot.Get<WelcomeService>(out var welcome);
            bot.Get<GuildService>(out var guild);
            bot.Get<EventService>(out var evt);
            bot.Get<AutoroleService>(out var autorole);
            bot.Get<LoggingService>(out var logger);
            return Executor.ExecuteAsync(() =>
            {
                bot.GuildAvailable += async args => await guild.OnAvailableAsync(args);
                bot.JoinedGuild += async args => await guild.OnJoinAsync(args);
                bot.LeftGuild += async args => await guild.OnLeaveAsync(args);
                
                bot.MemberJoined += async args =>
                {
                    if (Config.EnabledFeatures.Welcome)
                        await welcome.JoinAsync(args);
                    if (Config.EnabledFeatures.Autorole)
                        await autorole.DoAsync(args);
                };
                bot.MemberLeft += async args =>
                {
                    if (Config.EnabledFeatures.Welcome)
                        await welcome.LeaveAsync(args);
                };
                
                bot.Ready += async args => await evt.OnReady(args);
                bot.Logger.MessageLogged += async (_, args) => await logger.DoAsync(new LogEventArgs(args));

                return Task.CompletedTask;
            });
        }

        public static Task<RestUserMessage> SendToAsync(this LocalEmbedBuilder e, IMessageChannel c) => c.SendMessageAsync(embed: e.Build());

        public static Task<RestUserMessage> SendToAsync(this LocalEmbed e, IMessageChannel c) => c.SendMessageAsync(embed: e);

        public static async Task<IUserMessage> SendToAsync(this LocalEmbedBuilder e, IMember u) =>
            await (await u.CreateDmChannelAsync()).SendMessageAsync(embed: e.Build());

        public static async Task<IUserMessage> SendToAsync(this LocalEmbed e, IMember u) =>
            await (await u.CreateDmChannelAsync()).SendMessageAsync(embed: e);

        public static LocalEmbedBuilder WithSuccessColor(this LocalEmbedBuilder e) => e.WithColor(Config.SuccessColor);

        public static LocalEmbedBuilder WithColor(this LocalEmbedBuilder e, uint rawColor)
            => e.WithColor(new Color(rawColor.Cast<int>()));

        public static LocalEmbedBuilder WithErrorColor(this LocalEmbedBuilder e) 
            => e.WithColor(Config.ErrorColor);

        public static LocalEmoji ToEmoji(this string str) => new LocalEmoji(str);

        public static async Task<bool> TryDeleteAsync(this IDeletable deletable, RestRequestOptions options = null)
        {
            try
            {
                await deletable.DeleteAsync(options);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetUnicodeUrl(this IEmoji emoji)
        {
            try
            {
                return
                    $"https://i.kuro.mu/emoji/512x512/{emoji.Cast<Emoji>()?.ToString().GetUnicodePoints().Select(x => x.ToString("x2")).Join('-')}.png";
            }
            catch (ArgumentNullException)
            {
                return string.Empty;
            }
        }

        public static async ValueTask<string> GetJumpUrlAsync(this IMessage message)
        {
            var c = await message.Client.GetChannelAsync(message.ChannelId);
            var channel = message.ChannelId;
            return $"https://discordapp.com/channels/{(c.Cast<ITextChannel>()?.GuildId is null ? "@me" : $"{(c as ITextChannel)?.GuildId}")}/{channel}/{message.Id}";
        }
    }
}