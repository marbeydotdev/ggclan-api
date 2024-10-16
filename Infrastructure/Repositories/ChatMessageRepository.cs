using System.Linq.Expressions;
using Domain.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatMessageRepository : GenericRepository<ClanMessage>, IChatMessageRepository
{
    public ChatMessageRepository(GgDbContext dbContext) : base(dbContext)
    {
        
    }

    public new async Task<Result<List<ClanMessage>>> GetAllAsync(Expression<Func<ClanMessage, bool>> predicate, int skip = 0, int limit = 10, Expression<Func<ClanMessage, object>>? orderBy = null, bool ascending = true)
    {
        var result = await GetAllQueryable(predicate, skip, limit, orderBy, ascending)
            .Include(m => m.ClanMember).ThenInclude(m => m.User)
            .ToListAsync();
        
        return Result.Ok(result);
    }
}