namespace Volte.Helpers
{
    /// <summary>
    ///     Helper class for markdown formatting.
    /// </summary>
    public static class FormatHelper    // more will be added eventually
    {
        public static string Code(string text, string language = null) 
            => language is null
                ? $"`{text}`"
                : $"```{language}\n{text}```";

        public static string Bold(string text) 
            => $"**{text}**";
    }
}
