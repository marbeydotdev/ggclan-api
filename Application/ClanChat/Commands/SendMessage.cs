using Application.Achievements.Services;
using Application.DTO;
using Application.Users.Services;
using Domain.Entities;
using Domain.Enums;
using FluentResults;
using Infrastructure.Repositories;
using MediatR;

namespace Application.ClanChat.Commands;

public class SendMessageCommand : IRequest<Result<ClanMessage>>
{
    public string NameIdentifier  { get; set; } = null!;
    public int ClanId { get; set; }
    public CreateMessageDto CreateMessageDto { get; set; } = null!;
}

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<ClanMessage>>
{
    private readonly IUserService _userService;
    private readonly IChatMessageRepository _clanMessageRepository;
    private readonly IClanRepository _clanRepository;
    private readonly IAchievementService _achievementService;

    public SendMessageCommandHandler(IUserService userService, IChatMessageRepository clanMessageRepository, IClanRepository clanRepository, IAchievementService achievementService)
    {
        _userService = userService;
        _clanMessageRepository = clanMessageRepository;
        _clanRepository = clanRepository;
        _achievementService = achievementService;
    }

    public async Task<Result<ClanMessage>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        var clanMember = await _clanRepository.GetClanMemberAsync(user.Id, request.ClanId);
        if (clanMember.IsFailed)
        {
            return Result.Fail(clanMember.Errors);
        }

        var message = new ClanMessage
        {
            ClanId = request.ClanId,
            ClanMemberId = clanMember.Value.Id,
            Message = request.CreateMessageDto.Message
        };
        
        var added = await _clanMessageRepository.AddAsync(message, true);
        await _achievementService.AddAchievementIfNotExists(user.Id, (int)EAchievements.FirstMessage);
        return Result.Ok(added.Value!); // returnEntity = true, so not null
    }
}