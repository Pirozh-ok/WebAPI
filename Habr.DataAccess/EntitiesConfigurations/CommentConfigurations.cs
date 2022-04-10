using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class COmmentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.Property("Id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property("Text")
                .IsRequired()
                .HasMaxLength(500)
                .HasDefaultValue("");

            builder.Property("CreateDate")
                .IsRequired()
                .HasDefaultValueSql("getdate()");
        }
    }
}
