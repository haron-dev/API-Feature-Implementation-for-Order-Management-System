using OrderManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderManagementSystem.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId);
        
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? notes = null);
        
        Task<OrderAnalytics> GetOrderAnalyticsAsync();
    }
}
