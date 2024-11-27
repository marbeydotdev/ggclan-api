using Domain.Entities;
using FluentResults;
using Infrastructure.Interfaces;

namespace Application.Clans.Services;

public class ClanService : IClanService
{
    private readonly IGenericRepository<ClanInvite> _clanInviteRepository;
    private readonly IClanRepository _clanRepository;
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly IGenericRepository<ClanMember> _clanMemberRepository;

    public ClanService(IChatMessageRepository chatMessageRepository, IClanRepository clanRepository, IGenericRepository<ClanInvite> clanInviteRepository, IGenericRepository<ClanMember> clanMemberRepository)
    {
        _chatMessageRepository = chatMessageRepository;
        _clanRepository = clanRepository;
        _clanInviteRepository = clanInviteRepository;
        _clanMemberRepository = clanMemberRepository;
    }

    public async Task<Result<Clan>> GetClan(int clanId, int userId)
    {
        if (!await UserHasAccessToClan(userId, clanId))
        {
            return Result.Fail("Unauthorized");
        }
        
        var clan = await _clanRepository.GetAsync(c => c.Id == clanId);
        
        return clan;
    }
    
    public async Task<Result<List<ClanMessage>>> GetClanMessages(int userId, int clanId, int skip, int limit, int? afterId = null)
    {
        if (!await UserHasAccessToClan(userId, clanId))
        {
            return Result.Fail("Access denied.");
        }
        
        Result<List<ClanMessage>> messages;

        if (afterId.HasValue)
        {
            messages = await _chatMessageRepository.GetAllAsync(
                m => m.ClanId == clanId && m.Id > afterId, skip, limit, m => m.Created, false);
        }
        else
        {
            messages = await _chatMessageRepository.GetAllAsync(
                m => m.ClanId == clanId, skip, limit, m => m.Created, false);
        }
        
        return messages;
    }

    public async Task<bool> UserHasAccessToClan(int userId, int clanId)
    {
        var access = await _clanMemberRepository.AnyAsync(m => m.UserId == userId && m.ClanId == clanId);
        return access;
    }

    public async Task<Result> UserCanSendInvite(int userId, int clanId)
    {
        if (await UserHasAccessToClan(userId, clanId))
        {
            return Result.Fail("Already in clan.");
        }

        if (!await _clanRepository.AnyAsync(c => c.Id == clanId && !c.Private))
        {
            return Result.Fail("Clan is private.");
        }

        if (await _clanInviteRepository.AnyAsync(c => c.ClanId == clanId && c.UserId == userId))
        {
            return Result.Fail("An invite has already been sent.");
        }
        
        return Result.Ok();
    }
}