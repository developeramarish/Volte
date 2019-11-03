using System;
using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Gommon;
using Qmmands;
using Volte.Core.Attributes;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class GuildParser : TypeParser<CachedGuild>
    {
        public override ValueTask<TypeParserResult<CachedGuild>> ParseAsync(
            Parameter parameter,
            string value,
            CommandContext context)
        {
            var ctx = context.Cast<VolteContext>();
            CachedGuild guild = default;

            var guilds = ctx.Bot.Guilds;

            if (ulong.TryParse(value, out var id))
                guild = guilds.Values.FirstOrDefault(x => x.Id == id);

            if (guild is null)
            {
                var match = guilds.Values.Where(x =>
                    x.Name.EqualsIgnoreCase(value)).ToList();
                if (match.Count > 1)
                    return TypeParserResult<CachedGuild>.Unsuccessful(
                        "Multiple guilds found with that name, try using its ID.");

                guild = match.FirstOrDefault();
            }

            return guild is null
                ? TypeParserResult<CachedGuild>.Unsuccessful("Guild not found.")
                : TypeParserResult<CachedGuild>.Successful(guild);
        }
    }
}