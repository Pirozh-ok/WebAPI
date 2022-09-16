﻿using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .IsRequired();

            builder.HasMany(p => p.PostsRatings)
               .WithOne(r => r.Post)
               .HasForeignKey(r => r.PostId)
               .OnDelete(DeleteBehavior.ClientCascade)
               .IsRequired();

            builder.HasMany(p => p.Images)
               .WithOne(i => i.Post)
               .HasForeignKey(i => i.PostId)
               .OnDelete(DeleteBehavior.ClientCascade)
               .IsRequired();

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
                .HasDefaultValueSql("getdate()");

            builder.Property("Updated")
                .HasDefaultValueSql("getdate()");

            builder.Property("IsPublished")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property("TotalRating")
                .HasDefaultValue(0);
        }
    }
}
