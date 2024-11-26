using Application.Clans.Services;
using Application.Users.Services;
using Domain.Entities;
using Domain.Enums;
using FluentResults;
using Infrastructure;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Clans.Queries;

public class GetInvitesCommand : IRequest<Result<List<ClanInvite>>>
{
    public string NameIdentifier { get; set; } = null!;
    public int ClanId { get; set; }
}

public class GetInvitesCommandHandler : IRequestHandler<GetInvitesCommand, Result<List<ClanInvite>>>
{
    private readonly IUserService _userService;
    private readonly IClanService _clanService;
    private readonly IClanRepository _clanRepository;
    private readonly GgDbContext _context;
    
    public GetInvitesCommandHandler(IUserService userService, IClanService clanService, IClanRepository clanRepository, GgDbContext dbContext)
    {
        _userService = userService;
        _clanService = clanService;
        _clanRepository = clanRepository;
        _context = dbContext;
    }

    public async Task<Result<List<ClanInvite>>> Handle(GetInvitesCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        var clan = await _clanService.GetClan(request.ClanId, user.Id);
        if (clan.IsFailed)
        {
            return Result.Fail("Could not get clan");
        }

        var clanMember = await _clanRepository.GetClanMemberAsync(user.Id, request.ClanId);

        if (clanMember.IsFailed || clanMember.Value.Role == ClanMemberRole.Member)
        {
            return Result.Fail("You do not have the necessary permissions to view this clan.");
        }
        
        var invites = await _context.ClanInvites.Include(u => u.User).Where(c => c.ClanId == clan.Value.Id).ToListAsync();
        
        return Result.Ok(invites);
    }
}