using Application.Services;
using Application.Users.Services;
using Domain.Entities;
using Domain.Enums;
using FluentResults;
using Infrastructure.Repositories;
using MediatR;
using WebAPI.DTO;

namespace Application.Clans.Commands;

public class CreateClanCommand : IRequest<Result<Clan>>
{
    public CreateClanDto CreateClanDto { get; set; } = null!;
    public string NameIdentifier  { get; set; } = null!;
}

public class CreateClanCommandHandler : IRequestHandler<CreateClanCommand, Result<Clan>>
{
    private readonly UserService _userService;
    private readonly ClanRepository _clanRepository;
    private readonly AchievementService _achievementService;

    public CreateClanCommandHandler(UserService userService, ClanRepository clanRepository, IMediator mediator, AchievementService achievementService)
    {
        _userService = userService;
        _clanRepository = clanRepository;
        _achievementService = achievementService;
    }

    public async Task<Result<Clan>> Handle(CreateClanCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);

        var newClan = new Clan
        {
            Name = request.CreateClanDto.Name,
            Description = request.CreateClanDto.Description,
            Game = request.CreateClanDto.Game,
            Members =
            [
                new ClanMember
                {
                    Role = ClanMemberRole.Owner,
                    UserId = user.Id
                }
            ],
            Invites = []
        };
        
        var result = await _clanRepository.AddAsync(newClan, true);
        await _achievementService.AddAchievementIfNotExists(user.Id, (int)EAchievements.ClanCreated);
        
        return result as Result<Clan>; // returnEntity = true, so not null
    }
}