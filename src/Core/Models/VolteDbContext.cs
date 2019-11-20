using System.Linq;
using Disqord;
using Microsoft.EntityFrameworkCore;
using Volte.Core.Models.Guild;

namespace Volte.Core.Models
{
    public class VolteDbContext : DbContext
    {
        public DbSet<GuildData> Guilds;

        public GuildData Retrieve(Snowflake id) 
            => Guilds.FirstOrDefault(x => x.Id == id.RawValue);

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=data/Volte.db");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<GuildData>()
                .Property(g => g.Id)
                .IsRequired();
        }
    }
}
