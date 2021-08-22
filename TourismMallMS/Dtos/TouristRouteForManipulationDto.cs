using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.ValidationAttributes;

namespace TourismMallMS.Dtos
{
    [TouristRouteTitleMustBeDifferentFromDescriptionAttribute]
    public abstract class TouristRouteForManipulationDto
    {
        [Required(ErrorMessage = "title不可为空")]
        [MaxLength(100)]
        public string Title { get; set; }

        public decimal Price { get; set; }

        [Required(ErrorMessage = "description不可为空")]
        [MaxLength(1500)]
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public string Features { get; set; }
        public string Fees { get; set; }
        public string Notes { get; set; }
        public double? Rating { get; set; }
        public DateTime? UptateTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string TravelDays { get; set; }
        public string TripType { get; set; }
        public string DepartureCity { get; set; }
        public ICollection<TouristRoutePictureForCreationDto> TouristRoutePictures { get; set; }
            = new List<TouristRoutePictureForCreationDto>();
    }
}
