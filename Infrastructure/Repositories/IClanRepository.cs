using Domain.Entities;
using FluentResults;

namespace Infrastructure.Repositories;

public interface IClanRepository : IGenericRepository<Clan>
{
    public Task<Result<ClanMember>> GetClanMemberAsync(int userId, int clanId);
    public Task<Result<List<ClanMember>>> GetClanMembersAsync(int clanId);
}