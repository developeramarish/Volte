﻿using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord.Automation
{
    public class Autorole
    {
        public static async Task Apply(SocketGuildUser user)
        {
            var config = ServerConfig.Get(user.Guild);
            if (!string.IsNullOrEmpty(config.Autorole))
            {
                var targetRole = user.Guild.Roles.FirstOrDefault(r => r.Name == config.Autorole);
                await user.AddRoleAsync(targetRole);
            }
        }
    }
}