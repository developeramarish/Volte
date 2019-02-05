﻿using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Extensions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.General {
    public partial class GeneralModule : VolteModule {
        [Command("Suggest")]
        [Summary("Suggest features for Volte.")]
        [Remarks("Usage: |prefix|suggest")]
        public async Task Suggest() {
            await Context.CreateEmbed("You can suggest bot features [here](https://goo.gl/forms/i6pgYTSnDdMMNLZU2).")
                .SendTo(Context.Channel);
        }
    }
}