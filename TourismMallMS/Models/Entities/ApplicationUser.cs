﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismMallMS.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public ICollection<Order> Orders { get; set; }
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<string>>  Tokens { get; set; }
    }
}
