using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.EntitiesConfigurations;
using Habr.DataAccess.Entities;
using Microsoft.Extensions.Configuration;

namespace Habr.DataAccess
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration _configuration; 
        public DataContext(DbContextOptions<DataContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DataContext() 
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostRating> PostsRatings { get; set; }
        public DbSet<AvatarImage> AvatarImages { get; set; }
        public DbSet<PostImage> PostImages { get; set; }
        public DbSet<UserSubscriptions> UserSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration(_configuration));
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new AvatarImageConfiguration(_configuration));
            modelBuilder.ApplyConfiguration(new PostImageConfiguration());
            modelBuilder.ApplyConfiguration(new RatingConfiguration());
            modelBuilder.ApplyConfiguration(new UserSubscriptionsConfiguration());
        }
    }
}
