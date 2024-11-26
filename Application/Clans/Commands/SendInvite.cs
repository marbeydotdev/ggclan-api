using Application.Clans.Services;
using Application.Users.Services;
using Domain.Entities;
using FluentResults;
using Infrastructure;
using MediatR;

namespace Application.Clans.Commands;

public class SendInviteCommand : IRequest<Result>
{
    public string NameIdentifier { get; set; } = null!;
    public int ClanId { get; set; }
}

public class SendInviteCommandHandler : IRequestHandler<SendInviteCommand, Result>
{
    private readonly IClanService _clanService;
    private readonly IUserService _userService;
    private readonly GgDbContext _context;

    public SendInviteCommandHandler(IClanService clanService, GgDbContext context, IUserService userService)
    {
        _clanService = clanService;
        _context = context;
        _userService = userService;
    }

    public async Task<Result> Handle(SendInviteCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        var canSendInvite = await _clanService.UserCanSendInvite(user.Id, request.ClanId);

        if (canSendInvite.IsFailed)
        {
            return Result.Fail("Cannot join this clan.");
        }

        var invite = new ClanInvite
        {
            ClanId = request.ClanId,
            User = user
        };
        
        _context.ClanInvites.Add(invite);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}