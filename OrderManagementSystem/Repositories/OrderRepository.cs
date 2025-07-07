using OrderManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagementSystem.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly InMemoryRepository<Order, int> _repository;

        public OrderRepository()
        {
            _repository = new InMemoryRepository<Order, int>(
                order => order.Id,
                order => order.Id,
                (order, id) => order.Id = id
            );
        }


        public Task<Order?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }


        public Task<IEnumerable<Order>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }


        public Task<IEnumerable<Order>> FindAsync(System.Linq.Expressions.Expression<Func<Order, bool>> predicate)
        {
            return _repository.FindAsync(predicate);
        }


        public async Task<Order> AddAsync(Order entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Set order properties
            entity.OrderDate = DateTime.UtcNow;
            entity.Status = OrderStatus.Created;

            // Add initial status history
            entity.StatusHistory.Add(new OrderStatusHistory
            {
                OrderId = entity.Id,
                Status = OrderStatus.Created,
                ChangedDate = DateTime.UtcNow,
                Notes = "Order created"
            });

            return await _repository.AddAsync(entity);
        }


        public Task<Order> UpdateAsync(Order entity)
        {
            return _repository.UpdateAsync(entity);
        }


        public Task<bool> RemoveAsync(Order entity)
        {
            return _repository.RemoveAsync(entity);
        }


        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await FindAsync(o => o.CustomerId == customerId);
        }


        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await FindAsync(o => o.Status == status);
        }


        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await FindAsync(o => o.OrderDate >= startDate && o.OrderDate <= endDate);
        }


        public async Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? notes = null)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found");

            // Validate status transition
            if (!IsValidStatusTransition(order.Status, newStatus))
                throw new InvalidOperationException($"Invalid status transition from {order.Status} to {newStatus}");

            // Update order status
            order.Status = newStatus;

            // Add status history entry
            var historyEntry = new OrderStatusHistory
            {
                Id = order.StatusHistory.Count + 1,
                OrderId = order.Id,
                Status = newStatus,
                ChangedDate = DateTime.UtcNow,
                Notes = notes
            };
            order.StatusHistory.Add(historyEntry);

            // If order is delivered, set completion date
            if (newStatus == OrderStatus.Delivered)
            {
                order.CompletionDate = DateTime.UtcNow;
            }

            return await UpdateAsync(order);
        }


        public async Task<OrderAnalytics> GetOrderAnalyticsAsync()
        {
            var orders = await GetAllAsync();
            var ordersList = orders.ToList();

            var analytics = new OrderAnalytics
            {
                TotalOrders = ordersList.Count,
                CompletedOrders = ordersList.Count(o => o.Status == OrderStatus.Delivered),
                OrdersByStatus = new Dictionary<OrderStatus, int>()
            };

            // Calculate average order value
            if (ordersList.Any())
            {
                analytics.AverageOrderValue = ordersList.Average(o => o.Total);
            }

            // Calculate average fulfillment time
            var completedOrders = ordersList.Where(o => o.Status == OrderStatus.Delivered && o.CompletionDate.HasValue).ToList();
            if (completedOrders.Any())
            {
                var totalFulfillmentTime = TimeSpan.Zero;
                foreach (var order in completedOrders)
                {
                    totalFulfillmentTime += order.CompletionDate!.Value - order.OrderDate;
                }
                analytics.AverageFulfillmentTime = TimeSpan.FromTicks(totalFulfillmentTime.Ticks / completedOrders.Count);
            }

            // Count orders by status
            foreach (OrderStatus status in Enum.GetValues(typeof(OrderStatus)))
            {
                analytics.OrdersByStatus[status] = ordersList.Count(o => o.Status == status);
            }

            return analytics;
        }

        // Helper method to validate status transitions
        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (OrderStatus.Created, OrderStatus.Processing) => true,
                (OrderStatus.Created, OrderStatus.Cancelled) => true,
                (OrderStatus.Processing, OrderStatus.Shipped) => true,
                (OrderStatus.Processing, OrderStatus.Cancelled) => true,
                (OrderStatus.Shipped, OrderStatus.Delivered) => true,
                (OrderStatus.Shipped, OrderStatus.Cancelled) => true,
                _ => false
            };
        }
    }
}
