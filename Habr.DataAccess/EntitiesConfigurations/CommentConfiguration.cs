using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasOne(pc => pc.Parent)
                .WithMany(sc => sc.SubComments)
                .HasForeignKey(sc => sc.ParentId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .IsRequired(false);

            builder.Property("Id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property("Text")
                .IsRequired()
                .HasMaxLength(500)
                .HasDefaultValue("");

            builder.Property("CreateDate")
                .HasDefaultValueSql("getdate()");
        }
    }
}
