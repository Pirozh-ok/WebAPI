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
            /*builder.HasKey(u => u.Id);

            builder.Property("Id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property("PathImage")
                .IsRequired()
                .HasDefaultValue(_configuration["Content:PathDefaultAvatar"]);

            builder.Property("LoadDate")
                .IsRequired()
                .HasDefaultValueSql("getdate()");*/
        }
    }
}
