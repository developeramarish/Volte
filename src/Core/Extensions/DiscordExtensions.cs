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
            return HasRole(user, db.GetData(user.Guild).Configuration.Moderation.ModRole) ||
                   IsAdmin(user, provider) ||
                   IsGuildOwner(user);
        }

        private static bool HasRole(this CachedMember user, ulong roleId)
            => user.Roles.Select(x => x.Value.Id).Contains(new Snowflake(roleId));

        public static bool IsAdmin(this CachedMember user, IServiceProvider provider)
        {
            provider.Get<DatabaseService>(out var db);
            return HasRole(user,
                       db.GetData(user.Guild).Configuration.Moderation.AdminRole) ||
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

        public static Task RegisterVolteEventHandlersAsync(this VolteBot client, IServiceProvider provider)
        {
            provider.Get<WelcomeService>(out var welcome);
            provider.Get<GuildService>(out var guild);
            provider.Get<EventService>(out var evt);
            provider.Get<AutoroleService>(out var autorole);
            provider.Get<LoggingService>(out var logger);
            return Executor.ExecuteAsync(() =>
            {
                client.JoinedGuild += args => guild.OnJoinAsync(args);
                client.LeftGuild += args => guild.OnLeaveAsync(args);
                
                client.MemberJoined += async args =>
                {
                    if (Config.EnabledFeatures.Welcome)
                        await welcome.JoinAsync(args);
                    if (Config.EnabledFeatures.Autorole)
                        await autorole.DoAsync(args);
                };
                client.MemberLeft += async args =>
                {
                    if (Config.EnabledFeatures.Welcome)
                        await welcome.LeaveAsync(args);
                };
                
                client.Ready += args => evt.OnShardReady(args);
                client.MessageReceived += async args =>
                {
                    if (!(args.Message is CachedUserMessage msg) || msg.Author.IsBot) return;
                    if (msg.Channel is IDmChannel dmc)
                    {
                        await dmc.SendMessageAsync("Currently, I do not support commands via DM.");
                        return;
                    }

                    await evt.HandleMessageAsync(args);
                };
                client.Logger.MessageLogged += (_, args) =>
                {

                };

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
    }
}