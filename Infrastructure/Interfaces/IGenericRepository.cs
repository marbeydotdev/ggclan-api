using System.Linq.Expressions;
using FluentResults;

namespace Infrastructure.Interfaces;

public interface IGenericRepository<T>
{
    public Task<Result<T>> GetAsync(Expression<Func<T, bool>> predicate);

    public Task<Result<List<T>>> GetAllAsync(Expression<Func<T, bool>> predicate, int skip = 0, int limit = 10,
        Expression<Func<T, object>>? orderBy = null, bool ascending = true);

    public Task<Result<T?>> AddAsync(T entity, bool returnEntity = false);
    public Task<Result<T?>> UpdateAsync(T entity, bool returnEntity = false);
    public Task<Result> DeleteAsync(T entity);
    public Task<Result> DeleteAsync(Expression<Func<T, bool>> predicate);
    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}