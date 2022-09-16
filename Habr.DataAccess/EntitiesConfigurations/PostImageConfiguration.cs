using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class PostImageConfiguration : IEntityTypeConfiguration<PostImage>
    {
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
                .HasDefaultValueSql("getdate()");
        }
    }
}

