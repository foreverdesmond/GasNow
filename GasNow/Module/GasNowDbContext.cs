using Microsoft.EntityFrameworkCore;

namespace GasNow.Module
{
    public class GasNowDbContext : DbContext
    {
        public GasNowDbContext(DbContextOptions<GasNowDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GasFee>()
                .HasKey(c => new { c.BlockNumber, c.NetworkID });
        }

        public DbSet<Chain> Chains { get; set; }
        public DbSet<ChainAPIUrl> ChainAPIUrls { get; set; }
        public DbSet<GasFee> GasFees { get; set; }
    }
}
