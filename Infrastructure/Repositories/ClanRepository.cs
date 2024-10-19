using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using FluentResults;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClanRepository : GenericRepository<Clan>, IClanRepository
{
    public ClanRepository(GgDbContext context) : base(context)
    {
        
    }

    public new async Task<Result<Clan>> GetAsync(Expression<Func<Clan, bool>> predicate)
    {
        var clan = await Context.Clans.FirstOrDefaultAsync(predicate);
        
        return clan == null ? 
            Result.Fail("Clan not found.") : 
            Result.Ok(clan);
    }

    public async Task<Result<ClanWithMember>> GetWithMembersAsync(Expression<Func<ClanWithMember, bool>> predicate)
    {
        var clan = await Context.ClanWithMembers.FirstOrDefaultAsync(predicate);
        
        return clan == null ? 
            Result.Fail("Clan not found.") : 
            Result.Ok(clan);
    }
    
    public new async Task<Result<List<Clan>>> GetAllAsync(Expression<Func<Clan, bool>> predicate, int skip = 0, int limit = 10,
        Expression<Func<Clan, object>>? orderBy = null, bool ascending = true)
    {
        var query = GetAllQueryable(predicate, skip, limit, orderBy, ascending);
        var clans = await query.ToListAsync();
        return Result.Ok(clans);
    }
    
    public async Task<Result<List<Clan>>> GetJoinableAsync(int userId, Expression<Func<Clan, bool>>? predicate = null, int skip = 0, int limit = 10)
    {
        if (predicate is null)
        {
            return Result.Ok(await Context.AvailableClans(userId).Skip(skip).Take(limit).ToListAsync());
        }
        else
        {
            return Result.Ok(await Context.AvailableClans(userId).Where(predicate).Skip(skip).Take(limit).ToListAsync());
        }
    }
    
    public async Task<Result<ClanMember>> GetClanMemberAsync(int userId, int clanId)
    {
        var role = await Context.ClanMembers
            .FirstOrDefaultAsync(c =>
                c.ClanId == clanId &&
                c.UserId == userId);
        
        return role == null ? 
            Result.Fail("User is not participating in this clan.") : 
            Result.Ok(role);
    }
    
    public async Task<Result<List<ClanMember>>> GetClanMembersAsync(int clanId)
    {
        var role = await Context.ClanMembers.Where(c => c.ClanId == clanId).Include(m => m.User).ToListAsync();
        
        return role.Count == 0 ? 
            Result.Fail("No members returned, clan does not exist.") : 
            Result.Ok(role);
    }

    public async Task<Result<ClanMember>> AddClanMemberAsync(int userId, int clanId, ClanMemberRole role)
    {
        if (await Context.ClanMembers.AnyAsync(c => c.ClanId == clanId && c.UserId == userId))
        {
            return Result.Fail("User is already participating in this clan.");
        }

        var member = await Context.ClanMembers.AddAsync(new ClanMember
        {
            ClanId = clanId,
            UserId = userId,
            Role = role
        });

        await Context.SaveChangesAsync();

        return Result.Ok(member.Entity);
    }
}