using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace StreamingTitles.Data.Model
{
    public class TitlesContext : DbContext
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;
        public TitlesContext(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("default");
        }

        public DbSet<Title> Collection { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<TitleCategory> TitleCategories { get; set; }
        public DbSet<TitlePlatform> TitlePlatform { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySQL(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TitleCategory
            modelBuilder.Entity<TitleCategory>()
                .HasKey(tc => new { tc.TitleId, tc.CategoryId });
            modelBuilder.Entity<TitleCategory>()
                .HasOne(tc => tc.Title)
                .WithMany(t => t.TitleCategory)
                .HasForeignKey(tc => tc.TitleId);
            modelBuilder.Entity<TitleCategory>()
                .HasOne(tc => tc.Category)
                .WithMany(t => t.TitleCategory)
                .HasForeignKey(tc => tc.CategoryId);
            // TitlePlatform
            modelBuilder.Entity<TitlePlatform>()
                .HasKey(tp => new { tp.TitleId, tp.PlatformId });
            modelBuilder.Entity<TitlePlatform>()
                .HasOne(tp => tp.Title)
                .WithMany(t => t.TitlePlatform)
                .HasForeignKey(tp => tp.TitleId);
            modelBuilder.Entity<TitlePlatform>()
                .HasOne(tp => tp.Platform)
                .WithMany(p => p.TitlePlatform)
                .HasForeignKey(tp => tp.PlatformId);

        }
    }

}
