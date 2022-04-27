using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class COmmentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            //one-to-many relationship between Comment and Comment
            builder.HasOne( pc => pc.Parent )
                .WithMany( sc => sc.SubComments )
                .HasForeignKey( sc => sc.ParentId )
                .OnDelete( DeleteBehavior.ClientCascade )
                .IsRequired( false );

            //Property configuration
            builder.Property( "Id" )
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property( "Text" )
                .IsRequired()
                .HasMaxLength( 500 )
                .HasDefaultValue( "" );

            builder.Property( "CreateDate" )
                .IsRequired()
                .HasDefaultValueSql( "getdate()" );
        }
    }
}
