using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private readonly IConfiguration _configuration;

        public UserConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(u => u.PostsRatings)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(u => u.Subscriptions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property("Id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property("Name")
                .IsRequired()
                .HasMaxLength(200)
                .HasDefaultValue("");

            builder.Property("Email")
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue("");

            builder.Property("RegistrationDate")
                .HasDefaultValueSql("getdate()");

            builder.Property("Password")
                .IsRequired()
                .HasDefaultValue("")
                .HasMaxLength(150);

            builder.Property("Role")
                .HasDefaultValue(Roles.User);

            builder.Property("RefreshToken")
                .IsRequired()
                .HasDefaultValue("");

            builder.Property("RefreshTokenExpirationDate")
                .HasDefaultValueSql("getdate()");
        }
    }
}
