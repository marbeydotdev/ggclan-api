using Application.Services;
using Application.Users.Services;
using Domain.Entities;
using FluentResults;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Clans.Queries;

public class GetClansQuery : IRequest<Result<List<Clan>>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int Skip { get; set; } = 0;
    public int Limit { get; set; } = 10;
}

public class GetClansQueryHandler : IRequestHandler<GetClansQuery, Result<List<Clan>>>
{
    private readonly GenericRepository<Clan> _clans;

    public GetClansQueryHandler(UserService userService, GenericRepository<Clan> clans)
    {
        _clans = clans;
    }

    public async Task<Result<List<Clan>>> Handle(GetClansQuery request, CancellationToken cancellationToken)
    {
        var clans = await _clans.GetAllAsync(clan => 
            !clan.Private && clan.Name.Contains(request.SearchTerm), 
            skip: request.Skip, limit: request.Limit);

        return clans;
    }
}