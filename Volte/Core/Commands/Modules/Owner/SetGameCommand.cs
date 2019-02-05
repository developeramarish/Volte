﻿using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Discord;
using Volte.Core.Extensions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("SetGame")]
        [Summary("Sets the bot's game (presence).")]
        [Remarks("Usage: $setgame {game}")]
        [RequireBotOwner]
        public async Task SetGame([Remainder] string game) {
            await VolteBot.Client.SetGameAsync(game);
            await Context.CreateEmbed($"Set the bot's game to **{game}**.").SendTo(Context.Channel);
        }
    }
}