﻿using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Volte.Core.Data.Objects;
using Volte.Core.Discord;
using Volte.Core.Services;

namespace Volte.Core.Runtime
{
    internal static class Program
    {
        private static async Task Main()
        {
            await InitVolteAsync();
            await VolteBot.StartAsync();
        }

        private static async Task InitVolteAsync()
        {
            Console.Title = "Volte";
            Console.CursorVisible = false;
            if (!Directory.Exists("data"))
            {
                await VolteBot.GetRequiredService<LoggingService>()
                    .Log(LogSeverity.Critical, LogSource.Volte,
                        "The \"data\" directory didn't exist, so I created it for you.");
                Directory.CreateDirectory("data");
            }
        }
    }
}