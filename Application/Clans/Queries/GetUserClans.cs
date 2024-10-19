using Application.Users.Services;
using Domain.Entities;
using FluentResults;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Clans.Queries;

public class GetUserClansQuery : IRequest<Result<List<Clan>>>
{
    public string NameIdentifier { get; set; } = null!;
    public string SearchTerm { get; set; } = string.Empty;
    public int Skip { get; set; } = 0;
    public int Limit { get; set; } = 10;
}

public class GetUserClansQueryHandler : IRequestHandler<GetUserClansQuery, Result<List<Clan>>>
{
    private readonly IClanRepository _clans;
    private readonly IGenericRepository<ClanMember> _clanMembers;
    private readonly IUserService _userService;

    public GetUserClansQueryHandler(IClanRepository clans, IUserService userService, IGenericRepository<ClanMember> clanMembers)
    {
        _clans = clans;
        _userService = userService;
        _clanMembers = clanMembers;
    }

    public async Task<Result<List<Clan>>> Handle(GetUserClansQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        
        var participating = await _clanMembers.GetAllAsync(m => m.UserId == user.Id, skip: request.Skip, limit: request.Limit);

        if (!participating.IsSuccess)
        {
            return Result.Fail(participating.Errors);
        }

        var clanIds = participating.Value.Select(p => p.ClanId);
        
        var clans = await _clans.GetAllAsync(clan => clanIds.Contains(clan.Id) && clan.Name.Contains(request.SearchTerm));

        return clans;
    }
}