using Application.Clans.Services;
using Application.Services;
using Application.Users.Services;
using Domain.Entities;
using FluentResults;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Clans.Queries;

public class GetClanQuery : IRequest<Result<Clan>>
{
    public int ClanId { get; set; }
    public string? NameIdentifier  { get; set; } // if not null, authenticated request
}

public class GetClanQueryHandler : IRequestHandler<GetClanQuery, Result<Clan>>
{
    private readonly ClanService _clanService;
    private readonly UserService _userService;
    private readonly ClanRepository _clanRepository;

    public GetClanQueryHandler(ClanService clanService, UserService userService, ClanRepository clanRepository)
    {
        _clanService = clanService;
        _userService = userService;
        _clanRepository = clanRepository;
    }

    public async Task<Result<Clan>> Handle(GetClanQuery request, CancellationToken cancellationToken)
    {
        Result<Clan> query;
        
        if (request.NameIdentifier != null)
        {
            var user = await _userService.GetOrCreateUser(request.NameIdentifier);
            query = await _clanService.TryGetClan(request.ClanId, user.Id);
        }
        else
        {
            query = await _clanRepository.GetAsync(c => c.Id == request.ClanId);
        }

        return query;
    }
}