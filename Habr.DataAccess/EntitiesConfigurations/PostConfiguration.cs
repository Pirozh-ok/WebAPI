using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            //one-to-many relationship between Post and Comment
            builder.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .IsRequired();

            //Property configuration
            builder.Property("Id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property("Title")
                .IsRequired()
                .HasMaxLength(200)
                .HasDefaultValue("");

            builder.Property("Text")
                .IsRequired()
                .HasMaxLength(2000)
                .HasDefaultValue("");

            builder.Property("Created")
                .IsRequired()
                .HasDefaultValueSql("getdate()");

            builder.Property("Updated")
                .IsRequired()
                .HasDefaultValueSql("getdate()");

            builder.Property("IsPublished")
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
