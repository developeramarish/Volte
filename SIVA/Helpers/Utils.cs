﻿using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;

namespace SIVA.Helpers
{
    public class Utils
    {
        public static Embed CreateEmbed(SocketCommandContext ctx, string content)
        {
            var config = ServerConfig.Get(ctx.Guild);
            return new EmbedBuilder()
                .WithAuthor(ctx.Message.Author)
                .WithColor(new Color(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB))
                .WithDescription(content)
                .Build();
        }
    }
}