using Domain.Entities;
using FluentResults;

namespace Application.Clans.Services;

public interface IClanService
{
    public Task<Result<Clan>> GetClan(int clanId, int userId);
    public Task<Result<List<ClanMessage>>> GetClanMessages(int userId, int clanId, int skip, int limit);
    public Task<bool> UserHasAccessToClan(int userId, int clanId);
    public Task<Result> UserCanSendInvite(int userId, int clanId);
}