using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Services
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart> GetShoppingCartByUserIdAsync(string userId);
        Task CreateShoppingCartAsync(ShoppingCart shoppingCart);
        Task<bool> SaveAsync();
        Task AddShoppingCartItemAsync(LineItem lineItem);
        Task<LineItem> GetShoppingCartItemByItemIdAsync(int lineItemId);
        void DeleteShoppingCartItem(LineItem LineItem);
        Task<IEnumerable<LineItem>> GeshoppingCartItemsByIdListAsync(IEnumerable<int> ids);
        void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems);
    }
}
