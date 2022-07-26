﻿using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //one-to-many relationship between User and Post
            builder
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //one-to-many relationship between User and Comment
            builder
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey(u => u.Id);

            //Property configuration
            builder.Property("Id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property("Name")
                .IsRequired()
                .HasMaxLength(200)
                .HasDefaultValue("");

            builder.Property("Email")
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue("");

            builder.Property("RegistrationDate")
                .IsRequired()
                .HasDefaultValueSql("getdate()");
        }
    }
}
