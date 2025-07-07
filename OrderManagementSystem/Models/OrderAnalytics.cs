using System;
using System.Collections.Generic;

namespace OrderManagementSystem.Models
{
    public class OrderAnalytics
    {
        public decimal AverageOrderValue { get; set; }
        
        public TimeSpan AverageFulfillmentTime { get; set; }
        
        public double AverageFulfillmentTimeHours { get; set; }
        
        public int TotalOrders { get; set; }
        
        public int CompletedOrders { get; set; }
        
        public double CompletionRate { get; set; }
        
        public Dictionary<OrderStatus, int> OrdersByStatus { get; set; } = new Dictionary<OrderStatus, int>();
    }
}
