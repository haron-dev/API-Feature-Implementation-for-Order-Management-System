using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OrderManagementSystem.Repositories
{
    public class InMemoryRepository<T, TKey> : IRepository<T> where T : class
    {
        private readonly ConcurrentDictionary<TKey, T> _entities = new ConcurrentDictionary<TKey, T>();
        private readonly Func<T, TKey> _keySelector;
        private readonly Func<T, int> _idSelector;
        private readonly Action<T, int> _idSetter;
        private int _nextId = 1;

        public InMemoryRepository(
            Func<T, TKey> keySelector,
            Func<T, int> idSelector,
            Action<T, int> idSetter)
        {
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            _idSelector = idSelector ?? throw new ArgumentNullException(nameof(idSelector));
            _idSetter = idSetter ?? throw new ArgumentNullException(nameof(idSetter));
        }

        public Task<T?> GetByIdAsync(int id)
        {
            var entity = _entities.Values.FirstOrDefault(e => _idSelector(e) == id);
            return Task.FromResult(entity);
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(_entities.Values.AsEnumerable());
        }
        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var compiledPredicate = predicate.Compile();
            var entities = _entities.Values.Where(compiledPredicate);
            return Task.FromResult(entities);
        }

        public Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Set the ID if it's not already set
            if (_idSelector(entity) <= 0)
            {
                _idSetter(entity, _nextId++);
            }

            var key = _keySelector(entity);
            _entities.TryAdd(key, entity);
            return Task.FromResult(entity);
        }

        public Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var key = _keySelector(entity);
            _entities[key] = entity;
            return Task.FromResult(entity);
        }

        public Task<bool> RemoveAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var key = _keySelector(entity);
            return Task.FromResult(_entities.TryRemove(key, out _));
        }
    }
}
