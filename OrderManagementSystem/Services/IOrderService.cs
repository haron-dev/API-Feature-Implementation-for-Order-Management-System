using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services
{
    public interface IOrderService
    {
   
        Order? GetOrderById(int id);
        
        Task<Order?> GetOrderByIdAsync(int id);
        
        IEnumerable<Order> GetAllOrders();
        
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        
        Order CreateOrder(Order order);
        
        Task<Order> CreateOrderAsync(Order order);
        
        Order UpdateOrderStatus(int orderId, OrderStatus newStatus, string? notes = null);
        
        Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? notes = null);
        
        OrderAnalytics GetOrderAnalytics();
        
        Task<OrderAnalytics> GetOrderAnalyticsAsync();
    }

    // OrderAnalytics class moved to Models namespace
}
