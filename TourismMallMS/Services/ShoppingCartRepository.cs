using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Database;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Services
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly AppDbContext _context;
        public ShoppingCartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddShoppingCartItemAsync(LineItem lineItem)
        {
            await _context.LineItems.AddAsync(lineItem);
        }

        public async Task CreateShoppingCartAsync(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
        }

        public void DeleteShoppingCartItem(LineItem LineItem)
        {
            _context.LineItems.Remove(LineItem);
        }

        public void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems)
        {
            _context.LineItems.RemoveRange(lineItems);
        }

        public async Task<IEnumerable<LineItem>> GeshoppingCartItemsByIdListAsync(IEnumerable<int> ids)
        {
            return await _context.LineItems.Where(item => ids.Contains(item.Id)).ToListAsync();
        }

        public async Task<ShoppingCart> GetShoppingCartByUserIdAsync(string userId)
        {
            return await _context.ShoppingCarts
                .Include(s => s.User)
                .Include(s => s.ShoppingCartItems)
                .ThenInclude(li => li.TouristRoute)
                .Where(s => s.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<LineItem> GetShoppingCartItemByItemIdAsync(int lineItemId)
        {
            return await _context.LineItems
                .Where(item => item.Id == lineItemId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
