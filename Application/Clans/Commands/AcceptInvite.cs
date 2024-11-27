using Application.Users.Services;
using Domain.Entities;
using Domain.Enums;
using FluentResults;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Clans.Commands;

public class AcceptInviteCommand : IRequest<Result> {
    public string NameIdentifier  { get; set; } = null!;
    public int InviteId { get; set; }
}

public class AcceptInviteCommandHandler : IRequestHandler<AcceptInviteCommand, Result>
{
    private readonly IGenericRepository<ClanInvite> _clanInvites;
    private readonly IClanRepository _clanRepository;
    private readonly IUserService _userService;

    public AcceptInviteCommandHandler(IGenericRepository<ClanInvite> clanInvites, IClanRepository clanRepository, IUserService userService)
    {
        _clanInvites = clanInvites;
        _clanRepository = clanRepository;
        _userService = userService;
    }

    public async Task<Result> Handle(AcceptInviteCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        var invite = await _clanInvites.GetAsync(i => i.Id == request.InviteId);
        if (invite.IsFailed)
        {
            return Result.Fail("Invite not found.");
        }

        var clanMember = await _clanRepository.GetClanMemberAsync(user.Id, invite.Value.ClanId);

        if (clanMember.IsFailed || clanMember.Value.Role == ClanMemberRole.Member)
        {
            return Result.Fail("You need to be an administrator to accept this invite.");
        }

        await _clanRepository.AddClanMemberAsync(invite.Value.UserId, invite.Value.ClanId, ClanMemberRole.Member);

        await _clanInvites.DeleteAsync(invite.Value);
        return Result.Ok();
    }
}