using System;

namespace OrderManagementSystem.Models
{
    public class Discount
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DiscountType Type { get; set; }
        public decimal Value { get; set; } // Percentage or fixed amount
        public CustomerSegment? ApplicableSegment { get; set; } // Null means applicable to all segments
        public int? MinimumOrderCount { get; set; } // Minimum number of previous orders required
        public decimal? MinimumOrderValue { get; set; } // Minimum order value required
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }
}
