using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.EntitiesConfigurations;
using Microsoft.Extensions.Configuration;
using Habr.DataAccess.Entities;

namespace Habr.DataAccess
{
    public class DataContext : DbContext
    {
        private string _connectionString;
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            _connectionString = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(_connectionString, builder =>
            {
                builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostConfiguration).Assembly);
        }
    }
}
