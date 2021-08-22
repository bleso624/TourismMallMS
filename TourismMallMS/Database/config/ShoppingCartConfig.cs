using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Database.config
{
    public class ShoppingCartConfig : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.HasOne<ApplicationUser>(cart => cart.User)
                .WithOne(user => user.ShoppingCart)
                .HasForeignKey<ShoppingCart>(cart => cart.UserId);

            builder.HasMany<LineItem>(cart => cart.ShoppingCartItems)
                .WithOne()
                .HasForeignKey(item => item.ShoppingCartId);
        }
    }
}
