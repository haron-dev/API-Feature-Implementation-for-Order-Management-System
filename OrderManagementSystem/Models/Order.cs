using System;
using System.Collections.Generic;

namespace OrderManagementSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal SubTotal => Items.Sum(item => item.Price * item.Quantity);
        public decimal DiscountAmount { get; set; }
        public decimal Total => SubTotal - DiscountAmount;
        public DateTime? CompletionDate { get; set; }
        
        // For tracking status changes
        public List<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderStatusHistory
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime ChangedDate { get; set; }
        public string? Notes { get; set; }
    }

    public enum OrderStatus
    {
        Created,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
