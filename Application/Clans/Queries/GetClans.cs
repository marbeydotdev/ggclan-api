using Application.Users.Services;
using Domain.Entities;
using FluentResults;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Clans.Queries;

public class GetClansQuery : IRequest<Result<List<Clan>>>
{
    public string NameIdentifier  { get; set; } = null!;
    public string SearchTerm { get; set; } = string.Empty;
    public int Skip { get; set; } = 0;
    public int Limit { get; set; } = 10;
}

public class GetClansQueryHandler : IRequestHandler<GetClansQuery, Result<List<Clan>>>
{
    private readonly IClanRepository _clans;
    private readonly IUserService _users;

    public GetClansQueryHandler(IClanRepository clans, IUserService users)
    {
        _clans = clans;
        _users = users;
    }

    public async Task<Result<List<Clan>>> Handle(GetClansQuery request, CancellationToken cancellationToken)
    {
        var user = await _users.GetOrCreateUser(request.NameIdentifier);
        var clans = await _clans.GetJoinableAsync(user.Id, clan => clan.Name.Contains(request.SearchTerm) ,skip: request.Skip, limit: request.Limit);
        return clans;
    }
}