using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.EntitiesConfigurations
{
    public class UserSubscriptionsConfiguration : IEntityTypeConfiguration<UserSubscriptions>
    {
        public void Configure(EntityTypeBuilder<UserSubscriptions> builder)
        {
            builder.Property(us => us.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(us => us.UserId)
                .IsRequired();

            builder.Property(us => us.SubsUserId)
                .IsRequired();

            builder.Property(us => us.DateSubscribe)
                .HasDefaultValueSql("getdate()");
        }
    }
}
