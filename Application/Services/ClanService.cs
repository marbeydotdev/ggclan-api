using Domain.Entities;
using FluentResults;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ClanService
{
    private readonly GGDbContext _context;

    public ClanService(GGDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Clan>> TryGetClan(int clanId, int userId)
    {
        var clan = await _context.Clans
            .Include(c => c.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(clan => clan.Id == clanId);

        if (clan == null)
        {
            return Result.Fail("Clan not found");
        }

        if (clan.Members.All(m => m.User.Id != userId))
        {
            return Result.Fail("Unauthorized");
        }

        return Result.Ok(clan);
    }
    
    public async Task<Result<Clan>> TryGetClan(int clanId)
    {
        var clan = await _context.Clans
            .Include(c => c.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(clan => clan.Id == clanId);

        if (clan == null)
        {
            return Result.Fail("Clan not found");
        }
        
        return Result.Ok(clan);
    }

    public async Task<Result<IEnumerable<ClanMessage>>> TryGetClanMessages(int userId, int clanId, int skip, int limit)
    {
        var getClan = await TryGetClan(clanId, userId);
        if (getClan.IsFailed)
        {
            return Result.Fail<IEnumerable<ClanMessage>>(getClan.Errors);
        }

        var messages = await _context.ClanMessages
            .Where(message => message.ClanId == clanId)
            .OrderByDescending(message => message.Created)
            .Include(message => message.ClanMember)
            .Skip(skip)
            .Take(limit)
            .ToArrayAsync();

        return Result.Ok<IEnumerable<ClanMessage>>(messages);
    }
}