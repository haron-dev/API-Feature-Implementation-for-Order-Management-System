using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagementSystem.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
        }


        public async Task<decimal> CalculateDiscountAsync(Order order)
        {
            if (order == null || order.Customer == null)
                return 0;

            var applicableDiscounts = new List<Discount>();

            // Get discounts applicable to this customer's segment
            var segmentDiscounts = await _discountRepository.GetActiveDiscountsBySegmentAsync(order.Customer.Segment);
            applicableDiscounts.AddRange(segmentDiscounts);

            // Get discounts applicable to this customer's order count
            var orderCountDiscounts = await _discountRepository.GetActiveDiscountsByOrderCountAsync(order.Customer.TotalOrders);
            applicableDiscounts.AddRange(orderCountDiscounts);

            // Get discounts applicable to this order's value
            var orderValueDiscounts = await _discountRepository.GetActiveDiscountsByOrderValueAsync(order.SubTotal);
            applicableDiscounts.AddRange(orderValueDiscounts);

            // Calculate the best discount
            decimal bestDiscount = 0;
            foreach (var discount in applicableDiscounts.Distinct())
            {
                decimal currentDiscount = discount.Type == DiscountType.Percentage
                    ? order.SubTotal * discount.Value / 100
                    : discount.Value;

                bestDiscount = Math.Max(bestDiscount, currentDiscount);
            }

            return bestDiscount;
        }


        public decimal CalculateDiscount(Order order)
        {
            // Synchronous wrapper for the async method
            return CalculateDiscountAsync(order).GetAwaiter().GetResult();
        }


        public async Task<IEnumerable<Discount>> GetAllDiscountsAsync()
        {
            return await _discountRepository.GetAllActiveDiscountsAsync();
        }


        public IEnumerable<Discount> GetAllDiscounts()
        {
            // Synchronous wrapper for the async method
            return GetAllDiscountsAsync().GetAwaiter().GetResult();
        }


        public async Task<IEnumerable<Discount>> GetDiscountsBySegmentAsync(CustomerSegment segment)
        {
            return await _discountRepository.GetActiveDiscountsBySegmentAsync(segment);
        }


        public IEnumerable<Discount> GetDiscountsBySegment(CustomerSegment segment)
        {
            // Synchronous wrapper for the async method
            return GetDiscountsBySegmentAsync(segment).GetAwaiter().GetResult();
        }
    }
}
