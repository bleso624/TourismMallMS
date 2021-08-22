using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Database.config
{
    public class TouristRoutePictureConfig : IEntityTypeConfiguration<TouristRoutePicture>
    {
        public void Configure(EntityTypeBuilder<TouristRoutePicture> builder)
        {
            builder.Property(touristRoutePicture => touristRoutePicture.Url).HasMaxLength(100);
            builder.HasOne<TouristRoute>(touristRoutePicture => touristRoutePicture.TouristRoute)
                .WithMany(touristRoute => touristRoute.TouristRoutePictures)
                .HasForeignKey(touristRoutePicture => touristRoutePicture.TouristRouteId);
        }
    }
}
