using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Database.config
{
    public class TouristRouteConfig : IEntityTypeConfiguration<TouristRoute>
    {
        public void Configure(EntityTypeBuilder<TouristRoute> builder)
        {
            builder.Property(touristRoute => touristRoute.Title).IsRequired().HasMaxLength(100);
            builder.Property(touristRoute => touristRoute.Description).IsRequired().HasMaxLength(1500);
            builder.Property(touristRoute => touristRoute.OriginalPrice).HasColumnType("decimal(18, 2)");
        }
    }
}
