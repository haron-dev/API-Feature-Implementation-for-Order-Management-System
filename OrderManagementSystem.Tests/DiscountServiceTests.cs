using OrderManagementSystem.Models;
using OrderManagementSystem.Services;
using OrderManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using Xunit;
using Moq;

namespace OrderManagementSystem.Tests
{
    public class DiscountServiceTests
    {
        private readonly DiscountService _discountService;
        private readonly Mock<IDiscountRepository> _mockDiscountRepository;

        public DiscountServiceTests()
        {
            _mockDiscountRepository = new Mock<IDiscountRepository>();
            
            // Setup mock repository to return appropriate discounts for different segments
            var silverDiscount = new List<Discount> { new Discount { Type = DiscountType.Percentage, Value = 5 } };
            _mockDiscountRepository.Setup(repo => repo.GetActiveDiscountsBySegmentAsync(CustomerSegment.Silver))
                .ReturnsAsync(silverDiscount);
                
            var goldDiscount = new List<Discount> { new Discount { Type = DiscountType.Percentage, Value = 10 } };
            _mockDiscountRepository.Setup(repo => repo.GetActiveDiscountsBySegmentAsync(CustomerSegment.Gold))
                .ReturnsAsync(goldDiscount);
                
            var platinumDiscount = new List<Discount> { new Discount { Type = DiscountType.Percentage, Value = 15 } };
            _mockDiscountRepository.Setup(repo => repo.GetActiveDiscountsBySegmentAsync(CustomerSegment.Platinum))
                .ReturnsAsync(platinumDiscount);
                
            // Setup mock repository for order count discounts
            var loyalCustomerDiscount = new List<Discount> { new Discount { Type = DiscountType.FixedAmount, Value = 20 } };
            _mockDiscountRepository.Setup(repo => repo.GetActiveDiscountsByOrderCountAsync(It.Is<int>(count => count >= 10)))
                .ReturnsAsync(loyalCustomerDiscount);
                
            // Setup mock repository for order value discounts
            var mediumOrderDiscount = new List<Discount> { new Discount { Type = DiscountType.Percentage, Value = 7 } };
            _mockDiscountRepository.Setup(repo => repo.GetActiveDiscountsByOrderValueAsync(It.Is<decimal>(value => value >= 500 && value < 1000)))
                .ReturnsAsync(mediumOrderDiscount);
                
            var largeOrderDiscount = new List<Discount> { new Discount { Type = DiscountType.Percentage, Value = 30 } };
            _mockDiscountRepository.Setup(repo => repo.GetActiveDiscountsByOrderValueAsync(It.Is<decimal>(value => value >= 1000)))
                .ReturnsAsync(largeOrderDiscount);
                
            // Clear the default setup to avoid conflicts
            _mockDiscountRepository.Setup(repo => repo.GetActiveDiscountsBySegmentAsync(CustomerSegment.Regular))
                .ReturnsAsync(new List<Discount>());
            _mockDiscountRepository.Setup(repo => repo.GetActiveDiscountsByOrderCountAsync(0))
                .ReturnsAsync(new List<Discount>());
            _mockDiscountRepository.Setup(repo => repo.GetActiveDiscountsByOrderValueAsync(0))
                .ReturnsAsync(new List<Discount>());
                
            _discountService = new DiscountService(_mockDiscountRepository.Object);
        }

        [Fact]
        public void CalculateDiscount_RegularCustomer_NoDiscount()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                Name = "Regular Customer",
                Segment = CustomerSegment.Regular,
                TotalOrders = 2
            };

            var order = new Order
            {
                Id = 1,
                CustomerId = customer.Id,
                Customer = customer,
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new OrderItem { Id = 1, OrderId = 1, ProductName = "Product 1", Price = 100, Quantity = 1 }
                }
            };

            // Act
            decimal discount = _discountService.CalculateDiscount(order);

            // Assert
            Assert.Equal(0, discount);
        }

        [Fact]
        public void CalculateDiscount_SilverCustomer_AppliesPercentageDiscount()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 2,
                Name = "Silver Customer",
                Segment = CustomerSegment.Silver,
                TotalOrders = 5
            };

            var order = new Order
            {
                Id = 2,
                CustomerId = customer.Id,
                Customer = customer,
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new OrderItem { Id = 2, OrderId = 2, ProductName = "Product 2", Price = 100, Quantity = 1 }
                }
            };

            // Act
            decimal discount = _discountService.CalculateDiscount(order);

            // Assert - 5% discount on $100 = $5
            Assert.Equal(5, discount);
        }

        [Fact]
        public void CalculateDiscount_GoldCustomer_AppliesPercentageDiscount()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 3,
                Name = "Gold Customer",
                Segment = CustomerSegment.Gold,
                TotalOrders = 5
            };

            var order = new Order
            {
                Id = 3,
                CustomerId = customer.Id,
                Customer = customer,
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new OrderItem { Id = 3, OrderId = 3, ProductName = "Product 3", Price = 100, Quantity = 1 }
                }
            };

            // Act
            decimal discount = _discountService.CalculateDiscount(order);

            // Assert - 10% discount on $100 = $10
            Assert.Equal(10, discount);
        }

        [Fact]
        public void CalculateDiscount_PlatinumCustomer_AppliesPercentageDiscount()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 4,
                Name = "Platinum Customer",
                Segment = CustomerSegment.Platinum,
                TotalOrders = 5
            };

            var order = new Order
            {
                Id = 4,
                CustomerId = customer.Id,
                Customer = customer,
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new OrderItem { Id = 4, OrderId = 4, ProductName = "Product 4", Price = 100, Quantity = 1 }
                }
            };

            // Act
            decimal discount = _discountService.CalculateDiscount(order);

            // Assert - 15% discount on $100 = $15
            Assert.Equal(15, discount);
        }

        [Fact]
        public void CalculateDiscount_LoyalCustomer_AppliesFixedDiscount()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 5,
                Name = "Loyal Customer",
                Segment = CustomerSegment.Regular,
                TotalOrders = 15 // More than the minimum 10 orders
            };

            var order = new Order
            {
                Id = 5,
                CustomerId = customer.Id,
                Customer = customer,
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new OrderItem { Id = 5, OrderId = 5, ProductName = "Product 5", Price = 50, Quantity = 1 }
                }
            };

            // Act
            decimal discount = _discountService.CalculateDiscount(order);

            // Assert - Fixed $20 discount for loyal customers
            Assert.Equal(20, discount);
        }

        [Fact]
        public void CalculateDiscount_BigOrder_AppliesPercentageDiscount()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 6,
                Name = "Big Order Customer",
                Segment = CustomerSegment.Regular,
                TotalOrders = 2
            };

            var order = new Order
            {
                Id = 6,
                CustomerId = customer.Id,
                Customer = customer,
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new OrderItem { Id = 6, OrderId = 6, ProductName = "Expensive Product", Price = 600, Quantity = 1 }
                }
            };

            // Act
            decimal discount = _discountService.CalculateDiscount(order);

            // Assert - 7% discount on $600 = $42
            Assert.Equal(42, discount);
        }

        [Fact]
        public void CalculateDiscount_PlatinumCustomerWithBigOrder_AppliesBestDiscount()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 7,
                Name = "Platinum Big Order Customer",
                Segment = CustomerSegment.Platinum,
                TotalOrders = 5
            };

            var order = new Order
            {
                Id = 7,
                CustomerId = customer.Id,
                Customer = customer,
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new OrderItem { Id = 7, OrderId = 7, ProductName = "Expensive Product", Price = 600, Quantity = 1 }
                }
            };

            // Act
            decimal discount = _discountService.CalculateDiscount(order);

            // Assert - Should apply the best discount: 15% of $600 = $90 (better than 7% big order discount)
            Assert.Equal(90, discount);
        }
    }
}
