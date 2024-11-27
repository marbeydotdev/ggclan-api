using Application.Users.Services;
using Domain.Enums;
using FluentResults;
using Infrastructure;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Clans.Commands;

public class DenyInviteCommand : IRequest<Result>
{
    public string NameIdentifier { get; set; } = null!;
    public int InviteId { get; set; }
}

public class DenyInviteCommandHandler : IRequestHandler<DenyInviteCommand, Result>
{
    private readonly IUserService _userService;
    private readonly GgDbContext _context;
    private readonly IClanRepository _clanRepository;
    
    public DenyInviteCommandHandler(IUserService userService, GgDbContext context, IClanRepository clanRepository)
    {
        _userService = userService;
        _context = context;
        _clanRepository = clanRepository;
    }

    public async Task<Result> Handle(DenyInviteCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        var invite = await _context.ClanInvites.FirstOrDefaultAsync(i => i.Id == request.InviteId, cancellationToken);
        if (invite == null)
        {
            return Result.Fail("invite not found.");
        }

        var clanMember = await _clanRepository.GetClanMemberAsync(user.Id, invite.ClanId);

        if (clanMember.IsFailed || clanMember.Value.Role == ClanMemberRole.Member)
        {
            return Result.Fail("Access denied.");
        }
        
        _context.ClanInvites.Remove(invite);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}