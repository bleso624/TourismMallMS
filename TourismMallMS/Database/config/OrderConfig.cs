using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Database.config
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne<ApplicationUser>(order => order.User)
                .WithMany(user => user.Orders)
                .HasForeignKey(order => order.UserId);

            builder.Property(order => order.CreateDateUTC).IsRequired();
        }
    }
}
