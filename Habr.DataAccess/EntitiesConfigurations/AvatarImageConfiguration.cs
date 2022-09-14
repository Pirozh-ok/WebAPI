using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class AvatarImageConfiguration : IEntityTypeConfiguration<AvatarImage>
    {
        private readonly IConfiguration _configuration;

        public AvatarImageConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(EntityTypeBuilder<AvatarImage> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(a => a.PathImage)
                .IsRequired()
                .HasDefaultValue(_configuration["Content:PathDefaultAvatar"]);

            builder.Property(a => a.LoadDate)
                .IsRequired()
                .HasDefaultValueSql("getdate()");
        }
    }
}

