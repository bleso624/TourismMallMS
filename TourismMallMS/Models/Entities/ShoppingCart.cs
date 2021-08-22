using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismMallMS.Models.Entities
{
    public class ShoppingCart
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> ShoppingCartItems { get; set; }
    }
}
