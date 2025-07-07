using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories;
using OrderManagementSystem.Services;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace OrderManagementSystem.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Configure JSON serialization the same way as the main application
                services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    });
                    
                // Remove the existing repositories and services
                var orderRepoDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IOrderRepository));
                if (orderRepoDescriptor != null)
                {
                    services.Remove(orderRepoDescriptor);
                }

                var discountRepoDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDiscountRepository));
                if (discountRepoDescriptor != null)
                {
                    services.Remove(discountRepoDescriptor);
                }

                // Add mock repositories
                var mockOrderRepository = new Mock<IOrderRepository>();
                var mockDiscountRepository = new Mock<IDiscountRepository>();

                // Setup mock order repository
                var orders = new List<Order>
                {
                    new Order
                    {
                        Id = 1,
                        CustomerId = 1,
                        Customer = new Customer { Id = 1, Name = "Test Customer", Segment = CustomerSegment.Regular },
                        Status = OrderStatus.Created,
                        Items = new List<OrderItem>
                        {
                            new OrderItem { Id = 1, OrderId = 1, ProductName = "Test Product", Price = 100, Quantity = 1 }
                        },
                        StatusHistory = new List<OrderStatusHistory>
                        {
                            new OrderStatusHistory { Status = OrderStatus.Created, ChangedDate = System.DateTime.UtcNow }
                        }
                    }
                };

                mockOrderRepository.Setup(repo => repo.GetAllAsync())
                    .ReturnsAsync(orders);

                mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync((int id) => orders.FirstOrDefault(o => o.Id == id));

                mockOrderRepository.Setup(repo => repo.AddAsync(It.IsAny<Order>()))
                    .ReturnsAsync((Order order) =>
                    {
                        order.Id = orders.Count + 1;
                        order.Status = OrderStatus.Created;
                        order.StatusHistory = new List<OrderStatusHistory>
                        {
                            new OrderStatusHistory { Status = OrderStatus.Created, ChangedDate = System.DateTime.UtcNow }
                        };
                        orders.Add(order);
                        return order;
                    });

                mockOrderRepository.Setup(repo => repo.UpdateOrderStatusAsync(It.IsAny<int>(), It.IsAny<OrderStatus>(), It.IsAny<string>()))
                    .ReturnsAsync((int id, OrderStatus status, string notes) =>
                    {
                        var order = orders.FirstOrDefault(o => o.Id == id);
                        if (order != null)
                        {
                            order.Status = status;
                            order.StatusHistory.Add(new OrderStatusHistory
                            {
                                Status = status,
                                Notes = notes,
                                ChangedDate = System.DateTime.UtcNow
                            });
                        }
                        return order;
                    });

                mockOrderRepository.Setup(repo => repo.GetOrderAnalyticsAsync())
                    .ReturnsAsync(new OrderAnalytics
                    {
                        TotalOrders = orders.Count,
                        AverageOrderValue = orders.Count > 0 ? orders.Average(o => o.Items.Sum(i => i.Price * i.Quantity)) : 0,
                        CompletedOrders = orders.Count(o => o.Status == OrderStatus.Delivered),
                        CompletionRate = orders.Count > 0 ? (double)orders.Count(o => o.Status == OrderStatus.Delivered) / orders.Count : 0,
                        OrdersByStatus = new Dictionary<OrderStatus, int>()
                    });

                // Setup mock discount repository
                mockDiscountRepository.Setup(repo => repo.GetActiveDiscountsBySegmentAsync(It.IsAny<CustomerSegment>()))
                    .ReturnsAsync(new List<Discount>());

                mockDiscountRepository.Setup(repo => repo.GetActiveDiscountsByOrderCountAsync(It.IsAny<int>()))
                    .ReturnsAsync(new List<Discount>());

                mockDiscountRepository.Setup(repo => repo.GetActiveDiscountsByOrderValueAsync(It.IsAny<decimal>()))
                    .ReturnsAsync(new List<Discount>());

                // Register the mocks
                services.AddSingleton(mockOrderRepository.Object);
                services.AddSingleton(mockDiscountRepository.Object);

                // Make sure we're using the real services with our mock repositories
                services.AddSingleton<IDiscountService, DiscountService>();
                services.AddSingleton<IOrderService, OrderService>();
            });
        }
    }
}
