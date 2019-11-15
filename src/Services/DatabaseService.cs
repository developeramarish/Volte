using System;
using System.Collections.Generic;
using System.Linq;
using Disqord;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.Guild;

namespace Volte.Services
{
    public sealed class DatabaseService : VolteService
    {
        private static Func<GuildData, ulong, bool> _equals = (data, id) => data?.Id == id;

        private readonly VolteBot _bot;
        private readonly LoggingService _logger;

        public DatabaseService(VolteBot bot,
            LoggingService loggingService)
        {
            _bot = bot;
            _logger = loggingService;
        }

        public GuildData GetData(Snowflake id)
        {
            using (var db = new VolteDbContext())
            {
                var conf = db.Guilds.FirstOrDefault(x => x.Id == id.RawValue);
                if (conf is null)
                {
                    conf = Create(_bot.GetGuild(id));
                    db.Guilds.Add(conf);
                }

                return conf;
            }
        }

        public void ModifyData(CachedGuild guild, Action<GuildData> action) => ModifyData(guild.Id, action);

        public void ModifyData(Snowflake id, Action<GuildData> action)
        {
            try
            {
                _logger.Debug(LogSource.Volte, $"Getting data for guild {id}.");

                using (var db = new VolteDbContext())
                {
                    var conf = db.Guilds.FirstOrDefault(x => x.Id == id.RawValue);
                    if (conf is null)
                    {
                        conf = Create(_bot.GetGuild(id));
                        db.Guilds.Add(conf);
                    }

                    action.Invoke(conf);
                    db.SaveChanges();
                }

            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Service, e.Message, e);
            }

        }

        private static GuildData Create(CachedGuild guild)
            => new GuildData
            {
                Id = guild.Id,
                OwnerId = guild.OwnerId.RawValue,
                Configuration = new GuildConfiguration
                {
                    Autorole = default,
                    CommandPrefix = Config.CommandPrefix,
                    DeleteMessageOnCommand = default,
                    Moderation = new ModerationOptions
                    {
                        AdminRole = default,
                        Antilink = default,
                        Blacklist = new List<string>(),
                        MassPingChecks = default,
                        ModActionLogChannel = default,
                        ModRole = default
                    },
                    Welcome = new WelcomeOptions
                    {
                        LeavingMessage = string.Empty,
                        WelcomeChannel = default,
                        WelcomeColor = new Color(0x7000FB).RawValue,
                        WelcomeMessage = string.Empty
                    }
                },
                Extras = new GuildExtras
                {
                    ModActionCaseNumber = default,
                    SelfRoles = new List<string>(),
                    Tags = new List<Tag>(),
                    Warns = new List<Warn>()
                }
            };
    }
}