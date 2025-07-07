using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Models;
using OrderManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), 200)]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();
            
            return Ok(order);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Order), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(order);
                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Order>> UpdateOrderStatus(int id, [FromQuery] OrderStatus status, [FromQuery] string? notes = null)
        {
            try
            {
                var updatedOrder = await _orderService.UpdateOrderStatusAsync(id, status, notes);
                return Ok(updatedOrder);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("analytics")]
        [ProducesResponseType(typeof(OrderAnalytics), 200)]
        public async Task<ActionResult<OrderAnalytics>> GetOrderAnalytics()
        {
            var analytics = await _orderService.GetOrderAnalyticsAsync();
            return Ok(analytics);
        }
    }
}
