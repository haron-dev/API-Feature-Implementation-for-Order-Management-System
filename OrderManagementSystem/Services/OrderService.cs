using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderManagementSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDiscountService _discountService;

        public OrderService(IOrderRepository orderRepository, IDiscountService discountService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
        }


        public Order? GetOrderById(int id)
        {
            return GetOrderByIdAsync(id).GetAwaiter().GetResult();
        }


        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }


        public IEnumerable<Order> GetAllOrders()
        {
            return GetAllOrdersAsync().GetAwaiter().GetResult();
        }


        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }


        public Order CreateOrder(Order order)
        {
            return CreateOrderAsync(order).GetAwaiter().GetResult();
        }


        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.CustomerId <= 0)
                throw new ArgumentException("Customer ID must be provided");

            if (order.Items == null || order.Items.Count == 0)
                throw new ArgumentException("Order must contain at least one item");

            // Calculate discount
            order.DiscountAmount = await _discountService.CalculateDiscountAsync(order);

            // Add the order to the repository
            return await _orderRepository.AddAsync(order);
        }


        public Order UpdateOrderStatus(int orderId, OrderStatus newStatus, string? notes = null)
        {
            return UpdateOrderStatusAsync(orderId, newStatus, notes).GetAwaiter().GetResult();
        }


        public async Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? notes = null)
        {
            return await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, notes);
        }


        public OrderAnalytics GetOrderAnalytics()
        {
            return GetOrderAnalyticsAsync().GetAwaiter().GetResult();
        }


        public async Task<OrderAnalytics> GetOrderAnalyticsAsync()
        {
            return await _orderRepository.GetOrderAnalyticsAsync();
        }
    }
}
