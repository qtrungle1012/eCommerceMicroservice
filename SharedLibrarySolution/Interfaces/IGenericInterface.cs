using SharedLibrarySolution.Responses;
using System.Linq.Expressions;

namespace SharedLibrarySolution.Interfaces
{
    public interface IGenericInterface<T> where T : class
    {
        Task<Response> CreateAsync(T entity);
        Task<Response> UpdateAsync(T entity);
        Task<Response> DeleteAsync(T entity);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate);

    }
}
