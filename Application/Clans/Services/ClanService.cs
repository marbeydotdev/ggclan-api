using Domain.Entities;
using FluentResults;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Clans.Services;

public class ClanService : IClanService
{
    private readonly IGenericRepository<ClanInvite> _clanInviteRepository;
    private readonly IClanRepository _clanRepository;
    private readonly IChatMessageRepository _chatMessageRepository;

    public ClanService(IChatMessageRepository chatMessageRepository, IClanRepository clanRepository, IGenericRepository<ClanInvite> clanInviteRepository)
    {
        _chatMessageRepository = chatMessageRepository;
        _clanRepository = clanRepository;
        _clanInviteRepository = clanInviteRepository;
    }

    public async Task<Result<Clan>> GetClan(int clanId, int userId)
    {
        var clan = await _clanRepository
            .GetAsync(c => 
                c.Id == clanId && 
                c.Members.Any(m => m.UserId == userId));
        
        return clan;
    }
    
    public async Task<Result<List<ClanMessage>>> GetClanMessages(int userId, int clanId, int skip, int limit)
    {
        if (!await UserHasAccessToClan(userId, clanId))
        {
            return Result.Fail("Access denied.");
        }
        
        var messages = await _chatMessageRepository.GetAllAsync(
            m => m.ClanId == clanId, skip, limit, m => m.Created, false);

        return messages;
    }

    public async Task<bool> UserHasAccessToClan(int userId, int clanId)
    {
        var access = await _clanRepository
            .AnyAsync(c => c.Id == clanId && c.Members.Any(m => m.UserId == userId));

        return access;
    }

    public async Task<bool> UserCanSendInvite(int userId, int clanId)
    {
        var clanNotPrivateOrAlreadyMember = await _clanRepository
            .AnyAsync(c => c.Id == clanId && c.Members.All(m => m.UserId != userId) && !c.Private);

        if (!clanNotPrivateOrAlreadyMember)
        {
            return false;
        }
        
        var inviteAlreadySent = await _clanInviteRepository.AnyAsync(c => c.ClanId == clanId && c.UserId == userId);
        
        return !inviteAlreadySent;
    }
}