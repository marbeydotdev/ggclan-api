using System.Linq.Expressions;
using Domain.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClanRepository : GenericRepository<Clan>
{
    public ClanRepository(GgDbContext context) : base(context)
    {
        
    }

    public new async Task<Result<Clan>> GetAsync(Expression<Func<Clan, bool>> predicate)
    {
        var clan = await Context.Clans
            .Include(c => c.Members)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(predicate);
        
        return clan == null ? 
            Result.Fail("Clan not found.") : 
            Result.Ok(clan);
    }

    public new async Task<Result<List<Clan>>> GetAllAsync(Expression<Func<Clan, bool>> predicate, int skip = 0, int limit = 10,
        Expression<Func<Clan, object>>? orderBy = null, bool ascending = true)
    {
        var query = GetAllQueryable(predicate, skip, limit, orderBy, ascending);
        var clans = await query
            .Include(c => c.Members)
            .ToListAsync();
        return Result.Ok(clans);
    }
    
    public async Task<Result<ClanMember>> GetClanMemberAsync(int userId, int clanId)
    {
        var role = await Context.Clans
            .Where(c =>
                c.Id == clanId &&
                c.Members.Any(m => m.UserId == userId))
            .Select(c => c.Members.FirstOrDefault(m => m.UserId == userId)).SingleOrDefaultAsync();
        
        return role == null ? 
            Result.Fail("User is not participating in this clan.") : 
            Result.Ok(role);
    }
    
    public async Task<Result<List<ClanMember>>> GetClanMembersAsync(int clanId)
    {
        var role = await Context.Clans
            .Where(clan => clan.Id == clanId).SelectMany(c => c.Members).ToListAsync();
        
        return role.Count == 0 ? 
            Result.Fail("No members returned, clan does not exist.") : 
            Result.Ok(role);
    }
}