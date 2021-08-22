using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismMallMS.Models.Entities
{
    /// <summary>
    /// 旅游路线表
    /// </summary>
    public class TouristRoute
    {
        public Guid Id { get; set; }
        public decimal OriginalPrice { get; set; }
        public double? DiscountPresent { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public string Features { get; set; }
        public string Fees { get; set; }
        public string Notes { get; set; }
        public double? Rating { get; set; }
        public DateTime? UptateTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public TravelDays? TravelDays { get; set; }
        public TripType? TripType { get; set; }
        public DepartureCity? DepartureCity { get; set; }
        public ICollection<TouristRoutePicture> TouristRoutePictures { get; set; }
            = new List<TouristRoutePicture>();
    }
}
