using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OrderManagementSystem.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        
        Task<IEnumerable<T>> GetAllAsync();
        
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        Task<T> AddAsync(T entity);

        Task<T> UpdateAsync(T entity);

        Task<bool> RemoveAsync(T entity);
    }
}
