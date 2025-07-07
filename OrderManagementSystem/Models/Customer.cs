using System;

namespace OrderManagementSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CustomerSegment Segment { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int TotalOrders { get; set; }
    }

    public enum CustomerSegment
    {
        Regular,
        Silver,
        Gold,
        Platinum
    }
}
