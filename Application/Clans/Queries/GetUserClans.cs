using Application.Services;
using Application.Users.Services;
using Domain.Entities;
using FluentResults;
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
    private readonly GenericRepository<Clan> _clans;
    private readonly UserService _userService;

    public GetUserClansQueryHandler(UserService userService, GenericRepository<Clan> clans)
    {
        _userService = userService;
        _clans = clans;
    }

    public async Task<Result<List<Clan>>> Handle(GetUserClansQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        
        var clans = await _clans.GetAllAsync(clan => 
                !clan.Private && 
                clan.Name.Contains(request.SearchTerm) &&
                clan.Members.Any(member => member.UserId == user.Id)
            , 
            skip: request.Skip, limit: request.Limit);

        return clans;
    }
}