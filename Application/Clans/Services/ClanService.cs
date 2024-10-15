using Domain.Entities;
using FluentResults;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Clans.Services;

public class ClanService
{
    private readonly GenericRepository<ClanInvite> _clanInviteRepository;
    private readonly ClanRepository _clanRepository;
    private readonly ChatMessageRepository _chatMessageRepository;

    public ClanService(ClanRepository clanRepository, ChatMessageRepository chatMessageRepository, GenericRepository<ClanInvite> clanInviteRepository)
    {
        _clanRepository = clanRepository;
        _chatMessageRepository = chatMessageRepository;
        _clanInviteRepository = clanInviteRepository;
    }

    public async Task<Result<Clan>> TryGetClan(int clanId, int userId)
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