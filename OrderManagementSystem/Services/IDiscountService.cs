using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services
{
    public interface IDiscountService
    {
        decimal CalculateDiscount(Order order);
        
        Task<decimal> CalculateDiscountAsync(Order order);
        
        IEnumerable<Discount> GetAllDiscounts();
        
        Task<IEnumerable<Discount>> GetAllDiscountsAsync();
        
        IEnumerable<Discount> GetDiscountsBySegment(CustomerSegment segment);
        
        Task<IEnumerable<Discount>> GetDiscountsBySegmentAsync(CustomerSegment segment);
    }
}
