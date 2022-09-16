using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class RatingConfiguration : IEntityTypeConfiguration<PostRating>
    {
        public void Configure(EntityTypeBuilder<PostRating> builder)
        {
            builder.HasKey("Id");

            builder.Property("Id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property("Value")
                .HasDefaultValue(0);

            builder.Property("DateLastModified")
                .HasDefaultValueSql("getdate()");
        }
    }
}
