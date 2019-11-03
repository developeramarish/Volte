using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Disqord;
using Qmmands;
using Volte.Core.Attributes;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class EmoteParser : TypeParser<IEmoji>
    {
        public override ValueTask<TypeParserResult<IEmoji>> ParseAsync(
            Parameter param,
            string value,
            CommandContext context)
            => TryParseCustomEmoji(value, out var result)
                ? TypeParserResult<IEmoji>.Successful(result)
                : Regex.Match(value, "[^\u0000-\u007F]+", RegexOptions.IgnoreCase).Success
                    ? TypeParserResult<IEmoji>.Successful(new LocalEmoji(value))
                    : TypeParserResult<IEmoji>.Unsuccessful("Emote not found.");

        private bool TryParseCustomEmoji(string text, out LocalCustomEmoji result)
        {
            result = null;
            if (text.Length >= 4 && text[0] == '<' && (text[1] == ':' || (text[1] == 'a' && text[2] == ':')) &&
                text[text.Length - 1] == '>')
            {
                bool animated = text[1] == 'a';
                int startIndex = animated ? 3 : 2;

                int splitIndex = text.IndexOf(':', startIndex);
                if (splitIndex == -1)
                    return false;

                if (!ulong.TryParse(text.Substring(splitIndex + 1, text.Length - splitIndex - 2), NumberStyles.None,
                    CultureInfo.InvariantCulture, out ulong id))
                    return false;

                string name = text.Substring(startIndex, splitIndex - startIndex);
                result = new LocalCustomEmoji(id, name, animated);
                return true;
            }

            return false;
        }
    }
}