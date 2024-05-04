using AnimeStats.Service;
using Microsoft.EntityFrameworkCore;

namespace AnimeStats
{
    public class DatabaseEFCore : DbContext
    {
        public DatabaseEFCore(DbContextOptions<DatabaseEFCore> options) : base(options)
        {
        }
        public DbSet<Anime> Animes { get; set; }
        public DbSet<Season> Seasons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
