using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismMallMS.Models.Entities
{
    public class LineItem
    {
        public int Id { get; set; }
        public Guid TouristRouteId { get; set; }
        public TouristRoute TouristRoute { get; set; }
        public Guid ShoppingCartId { get; set; }
        public decimal OriginalPrice { get; set; }
        public double? DiscountPresent { get; set; }
    }
} 
