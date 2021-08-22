using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismMallMS.Helper;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Services
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task<bool> SaveAsync();
        Task<PaginationList<Order>> GetOrdersByUserIdAsync(string userId, int pageSize, int pageNumber);
        Task<Order> GetOrderByOrderIdAsync(Guid orderId);

    }
}
