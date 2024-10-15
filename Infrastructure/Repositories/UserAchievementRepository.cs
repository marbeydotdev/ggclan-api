using System.Linq.Expressions;
using Domain.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserAchievementRepository : GenericRepository<UserAchievement>
{
    public UserAchievementRepository(GgDbContext context) : base(context)
    {
    }
    
    public new async Task<Result<List<UserAchievement>>> GetAllAsync(Expression<Func<UserAchievement, bool>> predicate, int skip = 0, int limit = 10, Expression<Func<UserAchievement, object>>? orderBy = null, bool ascending = true)
    {
        var result = await GetAllQueryable(predicate, skip, limit, orderBy, ascending)
            .Include(u => u.Achievement)
            .ToListAsync();
        
        return Result.Ok(result);
    }
}