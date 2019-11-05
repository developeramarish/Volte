﻿using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Feedback", "Fb")]
        [Description("Submit feedback directly to the Volte guild.")]
        [Remarks("feedback {feedback}")]
        public Task<ActionResult> FeedbackAsync([Remainder] string feedback)
            => Ok($"Feedback sent! Message: ```{feedback}```", _ =>
                Context.CreateEmbedBuilder($"```{feedback}```")
                    .WithTitle($"Feedback from {Context.User}")
                    .SendToAsync(Context.Bot.GetPrimaryGuild().GetTextChannel(415182876326232064))
            );
    }
}