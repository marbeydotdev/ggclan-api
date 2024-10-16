using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using FluentResults;

namespace Infrastructure.Repositories;

public interface IClanRepository : IGenericRepository<Clan>
{
    public Task<Result<ClanMember>> GetClanMemberAsync(int userId, int clanId);
    public Task<Result<List<ClanMember>>> GetClanMembersAsync(int clanId);
    public Task<Result<ClanMember>> AddClanMemberAsync(int userId, int clanId, ClanMemberRole role);
    public Task<Result<ClanWithMember>> GetWithMembersAsync(Expression<Func<ClanWithMember, bool>> predicate);

    public Task<Result<List<Clan>>> GetJoinableAsync(int userId, Expression<Func<Clan, bool>>? predicate = null,
        int skip = 0, int limit = 10);

}