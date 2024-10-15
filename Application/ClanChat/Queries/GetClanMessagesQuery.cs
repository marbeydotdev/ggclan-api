using Application.Clans.Services;
using Application.Users.Services;
using Domain.Entities;
using FluentResults;
using MediatR;

namespace Application.ClanChat.Queries;

public class GetClanMessagesQuery : IRequest<Result<List<ClanMessage>>>
{
    public string NameIdentifier  { get; set; } = null!;
    public int ClanId { get; set; }
    public int Limit  { get; set; }
    public int Skip  { get; set; }
}

public class GetClanMessagesQueryHandler : IRequestHandler<GetClanMessagesQuery, Result<List<ClanMessage>>>
{
    private readonly IUserService _userService;
    private readonly IClanService _clanService;

    public GetClanMessagesQueryHandler(IUserService userService, IClanService clanService)
    {
        _userService = userService;
        _clanService = clanService;
    }

    public async Task<Result<List<ClanMessage>>> Handle(GetClanMessagesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        var messages = await _clanService.GetClanMessages(user.Id, request.ClanId, request.Skip, request.Limit);
        return messages;
    }
}