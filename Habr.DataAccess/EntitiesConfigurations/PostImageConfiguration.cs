/*using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class PostImageConfiguration : IEntityTypeConfiguration<PostImage>
    {
        private readonly IConfiguration _configuration;

        public PostImageConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(EntityTypeBuilder<PostImage> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(a => a.PathImage)
                .IsRequired()
                .HasDefaultValue("");

            builder.Property(a => a.LoadDate)
                .IsRequired()
                .HasDefaultValueSql("getdate()");
        }
    }
}
*/
