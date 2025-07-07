using OrderManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderManagementSystem.Repositories
{
    public interface IDiscountRepository : IRepository<Discount>
    {
        Task<IEnumerable<Discount>> GetActiveDiscountsBySegmentAsync(CustomerSegment segment);
        
        Task<IEnumerable<Discount>> GetActiveDiscountsByOrderCountAsync(int orderCount);
        
        Task<IEnumerable<Discount>> GetActiveDiscountsByOrderValueAsync(decimal orderValue);
        
        Task<IEnumerable<Discount>> GetAllActiveDiscountsAsync();
    }
}
