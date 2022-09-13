﻿using Habr.DataAccess.Entities;
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
            builder.HasKey(u => u.Id);

            builder.Property("Id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property("PathImage")
                .IsRequired()
                .HasDefaultValue("");

            builder.Property("LoadDate")
                .IsRequired()
                .HasDefaultValueSql("getdate()");
        }
    }
}
