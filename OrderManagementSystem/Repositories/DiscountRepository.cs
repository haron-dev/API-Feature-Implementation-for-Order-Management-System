using OrderManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagementSystem.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly InMemoryRepository<Discount, int> _repository;

        public DiscountRepository()
        {
            _repository = new InMemoryRepository<Discount, int>(
                discount => discount.Id,
                discount => discount.Id,
                (discount, id) => discount.Id = id
            );

            // Initialize with sample discounts
            InitializeSampleDiscounts();
        }

        private async void InitializeSampleDiscounts()
        {
            await AddAsync(new Discount
            {
                Name = "Silver Customer Discount",
                Description = "5% off for Silver customers",
                Type = DiscountType.Percentage,
                Value = 5,
                ApplicableSegment = CustomerSegment.Silver,
                IsActive = true
            });

            await AddAsync(new Discount
            {
                Name = "Gold Customer Discount",
                Description = "10% off for Gold customers",
                Type = DiscountType.Percentage,
                Value = 10,
                ApplicableSegment = CustomerSegment.Gold,
                IsActive = true
            });

            await AddAsync(new Discount
            {
                Name = "Platinum Customer Discount",
                Description = "15% off for Platinum customers",
                Type = DiscountType.Percentage,
                Value = 15,
                ApplicableSegment = CustomerSegment.Platinum,
                IsActive = true
            });

            await AddAsync(new Discount
            {
                Name = "Loyal Customer Discount",
                Description = "Fixed $20 discount for customers with 10+ orders",
                Type = DiscountType.FixedAmount,
                Value = 20,
                MinimumOrderCount = 10,
                IsActive = true
            });

            await AddAsync(new Discount
            {
                Name = "Big Order Discount",
                Description = "7% off for orders over $500",
                Type = DiscountType.Percentage,
                Value = 7,
                MinimumOrderValue = 500,
                IsActive = true
            });
        }


        public Task<Discount?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }


        public Task<IEnumerable<Discount>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }


        public Task<IEnumerable<Discount>> FindAsync(System.Linq.Expressions.Expression<Func<Discount, bool>> predicate)
        {
            return _repository.FindAsync(predicate);
        }


        public Task<Discount> AddAsync(Discount entity)
        {
            return _repository.AddAsync(entity);
        }


        public Task<Discount> UpdateAsync(Discount entity)
        {
            return _repository.UpdateAsync(entity);
        }


        public Task<bool> RemoveAsync(Discount entity)
        {
            return _repository.RemoveAsync(entity);
        }


        public async Task<IEnumerable<Discount>> GetActiveDiscountsBySegmentAsync(CustomerSegment segment)
        {
            var allDiscounts = await GetAllActiveDiscountsAsync();
            return allDiscounts.Where(d => d.ApplicableSegment == null || d.ApplicableSegment == segment);
        }


        public async Task<IEnumerable<Discount>> GetActiveDiscountsByOrderCountAsync(int orderCount)
        {
            var allDiscounts = await GetAllActiveDiscountsAsync();
            return allDiscounts.Where(d => d.MinimumOrderCount == null || orderCount >= d.MinimumOrderCount);
        }


        public async Task<IEnumerable<Discount>> GetActiveDiscountsByOrderValueAsync(decimal orderValue)
        {
            var allDiscounts = await GetAllActiveDiscountsAsync();
            return allDiscounts.Where(d => d.MinimumOrderValue == null || orderValue >= d.MinimumOrderValue);
        }


        public async Task<IEnumerable<Discount>> GetAllActiveDiscountsAsync()
        {
            var now = DateTime.UtcNow;
            return await FindAsync(d => d.IsActive &&
                                      (d.StartDate == null || d.StartDate <= now) &&
                                      (d.EndDate == null || d.EndDate >= now));
        }
    }
}
