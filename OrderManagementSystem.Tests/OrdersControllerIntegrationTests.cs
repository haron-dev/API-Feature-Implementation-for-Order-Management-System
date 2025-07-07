using Microsoft.AspNetCore.Mvc.Testing;
using OrderManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace OrderManagementSystem.Tests
{
    public class OrdersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public OrdersControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
            };
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedResponse()
        {
            // Arrange
            var order = new Order
            {
                CustomerId = 1,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductName = "Test Product",
                        Price = 100,
                        Quantity = 2
                    }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(order),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/orders", content);

            // Assert - Only check the status code for now
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            // Verify we have a Location header
            Assert.NotNull(response.Headers.Location);
        }

        [Fact]
        public async Task GetOrderAnalytics_ReturnsOkResponse()
        {
            // Act
            var response = await _client.GetAsync("/api/orders/analytics");

            // Assert - Only check the status code for now
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateOrderStatus_WithValidTransition_ReturnsOkResponse()
        {
            // Arrange - First create an order
            var order = new Order
            {
                CustomerId = 1,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductName = "Test Product",
                        Price = 100,
                        Quantity = 1
                    }
                }
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(order),
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync("/api/orders", createContent);
            
            // For now, just check that the create response is successful
            Assert.True(createResponse.IsSuccessStatusCode, $"Failed to create order: {await createResponse.Content.ReadAsStringAsync()}");
            
            // Extract the order ID from the location header
            var locationHeader = createResponse.Headers.Location;
            Assert.NotNull(locationHeader);
            var orderId = int.Parse(locationHeader.Segments.Last());
            
            // Act - Use query parameters instead of JSON body
            var response = await _client.PutAsync($"/api/orders/{orderId}/status?status={OrderStatus.Processing}&notes=Processing%20started", null);

            // Assert - Only check the status code for now
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
