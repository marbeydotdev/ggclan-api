using System.Linq.Expressions;
using Domain.Common;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GenericRepository<T>: IGenericRepository<T> where T : BaseEntity {
    protected readonly GgDbContext Context;
    
    private const int MaxLimit = 50;

    public GenericRepository(GgDbContext context)
    {
        Context = context;
    }

    public async Task<Result<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
        var result = await Context.Set<T>().FirstOrDefaultAsync(predicate);
        return result is null ? 
            Result.Fail($"The {typeof(T)} was not found.") : 
            Result.Ok(result);
    }

    public async Task<Result<List<T>>> GetAllAsync(Expression<Func<T, bool>> predicate, int skip = 0, int limit = 10, Expression<Func<T, object>>? orderBy = null, bool ascending = true)
    {
        var result = await GetAllQueryable(predicate, skip, limit, orderBy, ascending).ToListAsync();
        return Result.Ok(result);
    }
    
    public async Task<Result<T?>> AddAsync(T entity, bool returnEntity = false)
    {
        var entry = await Context.Set<T>().AddAsync(entity);
        await Context.SaveChangesAsync();
        
        return Result.Ok(returnEntity ? entry.Entity : null);
    }

    public async Task<Result<T?>> UpdateAsync(T entity, bool returnEntity = false)
    {
        var entry = Context.Set<T>().Update(entity);
        await Context.SaveChangesAsync();
        return Result.Ok(returnEntity ? entry.Entity : null);
    }

    public async Task<Result> DeleteAsync(T entity)
    {
        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        Context.Set<T>().RemoveRange(Context.Set<T>().Where(predicate));
        await Context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        var result = await Context.Set<T>().AnyAsync(predicate);
        return result;
    }
    
    protected IQueryable<T> GetAllQueryable(Expression<Func<T, bool>> predicate, int skip = 0, int limit = 10, Expression<Func<T, object>>? orderBy = null, bool ascending = true)
    {
        limit = Math.Clamp(limit, 1, MaxLimit);
        skip = skip < 0 ? 0 : skip;

        var query = orderBy == null ? 
            Context.Set<T>().Where(predicate).Skip(skip).Take(limit) :
            ascending ? 
                Context.Set<T>().Where(predicate).OrderBy(orderBy).Skip(skip).Take(limit) :
                Context.Set<T>().Where(predicate).OrderByDescending(orderBy).Skip(skip).Take(limit);

        query = ascending ? 
            query : 
            orderBy == null ? 
                query.OrderByDescending(t => t.Id) : 
                query.OrderByDescending(orderBy);

        return query;
    }
}