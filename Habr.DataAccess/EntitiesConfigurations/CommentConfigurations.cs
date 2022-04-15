using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class COmmentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            //many-to-one relationship between Comment and User
            builder
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            //many-to-one relationship between Comment and Post
            builder.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            //one-to-many relationship between Comment and Comment
            builder.HasOne(pc => pc.Parent)
                .WithMany(sc => sc.SubComments)
                .HasForeignKey(sc => sc.ParentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            //Property configuration
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
