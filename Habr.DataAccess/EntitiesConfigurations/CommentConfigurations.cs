using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class COmmentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder
                .HasMany(x => x.Comments)
                .WithOne(x => x.ParentComment)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

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
