using System;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Events;
using Qmmands;
using Gommon;
using Volte.Core;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Commands
{
    public sealed class VolteContext : DiscordCommandContext
    {
        private readonly EmojiService _emojiService;

        public static VolteContext Create(DiscordBot bot, string prefix, CachedUserMessage msg) 
            => new VolteContext(bot, prefix, msg);

        public static VolteContext FromMessageReceivedEventArgs(MessageReceivedEventArgs args) 
            => Create(args.Client.Cast<VolteBot>(), string.Empty, args.Message.Cast<CachedUserMessage>());

        // ReSharper disable once SuggestBaseTypeForParameter
        public VolteContext(DiscordBot bot, string prefix, CachedUserMessage msg) : base(bot, prefix, msg)
        {
            bot.Get(out _emojiService);
            bot.Get<DatabaseService>(out var db);
            Bot = bot;
            Guild = msg.Channel.Cast<CachedTextChannel>()?.Guild;
            Channel = msg.Channel.Cast<CachedTextChannel>();
            Member = msg.Author.Cast<CachedMember>();
            Message = msg;
            User = msg.Author.Cast<CachedMember>();
            GuildData = db.GetData(Guild);
            Now = DateTimeOffset.UtcNow;
        }

        public override DiscordBot Bot { get; }
        public override ICachedMessageChannel Channel { get; }
        public override CachedGuild Guild { get; }
        public override CachedMember Member { get; }
        public override CachedUserMessage Message { get; }
        public override CachedUser User { get; }
        public readonly GuildData GuildData;
        public readonly DateTimeOffset Now;

        public Task ReactFailureAsync() => Message.AddReactionAsync(_emojiService.X.ToEmoji());

        public Task ReactSuccessAsync() => Message.AddReactionAsync(_emojiService.X.ToEmoji());

        public LocalEmbed CreateEmbed(string content) => new LocalEmbedBuilder().WithSuccessColor().WithAuthor(User)
            .WithDescription(content).Build();

        public LocalEmbedBuilder CreateEmbedBuilder(string content = null) => new LocalEmbedBuilder()
            .WithSuccessColor().WithAuthor(User).WithDescription(content ?? string.Empty);

        public Task ReplyAsync(string content) => Channel.SendMessageAsync(content);

        public Task ReplyAsync(LocalEmbed embed) => embed.SendToAsync(Channel);

        public Task ReplyAsync(LocalEmbedBuilder embed) => embed.SendToAsync(Channel);

        public Task ReactAsync(string unicode) => Message.AddReactionAsync(new LocalEmoji(unicode));
    }
}