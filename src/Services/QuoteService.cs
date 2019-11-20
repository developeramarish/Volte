using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Events;
using Gommon;
using Volte.Commands;
using Volte.Core;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    //thanks MODiX for the idea and some of the code (definitely the regex lol)
    public class QuoteService : VolteEventService
    {
        private readonly VolteBot _bot;
        private readonly DatabaseService _db;

        public QuoteService(VolteBot bot,
            DatabaseService databaseService)
        {
            _bot = bot;
            _db = databaseService;
        }

        private static readonly Regex JumpUrlPattern = new Regex(
            @"(?<Prelink>\S+\s+\S*)?https?://(?:(?:ptb|canary)\.)?discordapp\.com/channels/(?<GuildId>\d+)/(?<ChannelId>\d+)/(?<MessageId>\d+)/?(?<Postlink>\S*\s+\S+)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public override Task DoAsync(EventArgs args)
            => OnMessageReceivedAsync(args.Cast<MessageReceivedEventArgs>());

        private async Task OnMessageReceivedAsync(MessageReceivedEventArgs args)
        {
            var ctx = VolteContext.Create(_bot, string.Empty, args.Message.Cast<CachedUserMessage>());
            if (!ctx.GuildData.Extras.AutoParseQuoteUrls) return;
            foreach (Match match in JumpUrlPattern.Matches(args.Message.Content))
            {
                if (ulong.TryParse(match.Groups["GuildId"].Value, out _)
                    && ulong.TryParse(match.Groups["ChannelId"].Value, out var channelId)
                    && ulong.TryParse(match.Groups["MessageId"].Value, out var messageId))
                {
                    var c = _bot.GetChannel(channelId);
                    if (c is ITextChannel channel)
                    {
                        var m = await channel.GetMessageAsync(messageId);
                        if (m is null) return;
                        await ctx.CreateEmbedBuilder()
                            .WithAuthor(m.Author)
                            .WithDescription($"```{m.Content}```")
                            .AddField("Quoted By", $"**{ctx.Member.DisplayName}#{ctx.Member.Discriminator}** in <#{ctx.Channel.Id}>.")
                            .SendToAsync(ctx.Channel);

                        if (match.Groups["Prelink"].Value.IsNullOrEmpty() &&
                            match.Groups["Postlink"].Value.IsNullOrEmpty())
                            _ = await args.Message.TryDeleteAsync();
                    }
                }
            }
        }
    }
}