using Microsoft.Extensions.Caching.Memory;
using OrderManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagementSystem.Services
{
    public class CachedDiscountService : IDiscountService
    {
        private readonly IDiscountService _discountService;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public CachedDiscountService(IDiscountService discountService, IMemoryCache cache)
        {
            _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }


        public decimal CalculateDiscount(Order order)
        {
            // For order-specific discount calculations, we don't cache as they're unique per order
            return _discountService.CalculateDiscount(order);
        }


        public async Task<decimal> CalculateDiscountAsync(Order order)
        {
            // For order-specific discount calculations, we don't cache as they're unique per order
            return await _discountService.CalculateDiscountAsync(order);
        }


        public IEnumerable<Discount> GetAllDiscounts()
        {
            string cacheKey = "all_discounts";

            // Try to get from cache first
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Discount>? discounts))
            {
                // Cache miss, get from the underlying service
                discounts = _discountService.GetAllDiscounts();

                // Store in cache
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_cacheDuration);

                _cache.Set(cacheKey, discounts, cacheOptions);
            }

            return discounts ?? Enumerable.Empty<Discount>();
        }


        public async Task<IEnumerable<Discount>> GetAllDiscountsAsync()
        {
            string cacheKey = "all_discounts";

            // Try to get from cache first
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Discount>? discounts))
            {
                // Cache miss, get from the underlying service
                discounts = await _discountService.GetAllDiscountsAsync();

                // Store in cache
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_cacheDuration);

                _cache.Set(cacheKey, discounts, cacheOptions);
            }

            return discounts ?? Enumerable.Empty<Discount>();
        }


        public IEnumerable<Discount> GetDiscountsBySegment(CustomerSegment segment)
        {
            string cacheKey = $"discounts_segment_{segment}";

            // Try to get from cache first
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Discount>? discounts))
            {
                // Cache miss, get from the underlying service
                discounts = _discountService.GetDiscountsBySegment(segment);

                // Store in cache
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_cacheDuration);

                _cache.Set(cacheKey, discounts, cacheOptions);
            }

            return discounts ?? Enumerable.Empty<Discount>();
        }


        public async Task<IEnumerable<Discount>> GetDiscountsBySegmentAsync(CustomerSegment segment)
        {
            string cacheKey = $"discounts_segment_{segment}";

            // Try to get from cache first
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Discount>? discounts))
            {
                // Cache miss, get from the underlying service
                discounts = await _discountService.GetDiscountsBySegmentAsync(segment);

                // Store in cache
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_cacheDuration);

                _cache.Set(cacheKey, discounts, cacheOptions);
            }

            return discounts ?? Enumerable.Empty<Discount>();
        }
    }
}
