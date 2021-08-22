using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Database;
using TourismMallMS.Helper;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<Order> GetOrderByOrderIdAsync(Guid orderId)
        {
            return await _context.Orders.Include(order => order.OrderItems)
                .ThenInclude(orderItem => orderItem.TouristRoute)
                .FirstOrDefaultAsync(order => order.Id == orderId);
        }

        public async Task<PaginationList<Order>> GetOrdersByUserIdAsync(string userId, int pageSize, int pageNumber)
        {
            var res = _context.Orders.Where(order => order.UserId == userId);
            return await PaginationList<Order>.CreateAsync(pageSize, pageNumber, res);
        }

        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }

}
