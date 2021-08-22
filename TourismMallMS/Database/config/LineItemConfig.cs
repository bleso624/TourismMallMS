using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Database.config
{
    public class LineItemConfig : IEntityTypeConfiguration<LineItem>
    {
        public void Configure(EntityTypeBuilder<LineItem> builder)
        {
            builder.HasOne<TouristRoute>(item => item.TouristRoute)
                .WithOne()
                .HasForeignKey<LineItem>(item => item.TouristRouteId);
        }
    }
}
