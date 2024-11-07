using System.Security.Claims;
using Application.Clans.Services;
using Application.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Controllers;

[Authorize]
[EnableCors("dev")]
public class RealtimeHub : Hub
{
    private readonly IUserService _userService;
    private readonly IClanService _clanService;

    public RealtimeHub(IUserService userService, IClanService clanService)
    {
        _userService = userService;
        _clanService = clanService;
    }

    public override async Task OnConnectedAsync()
    {
        var nameIdentifier = GetNameIdentifier(Context);
        var user = await _userService.GetOrCreateUser(nameIdentifier);
        await Groups.AddToGroupAsync(Context.ConnectionId, user.Id.ToString());
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var nameIdentifier = GetNameIdentifier(Context);
        var user = await _userService.GetOrCreateUser(nameIdentifier);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.Id.ToString());
        await base.OnDisconnectedAsync(exception);
    }

    public async Task StartClanChatListen(int clanId)
    {
        var nameIdentifier = GetNameIdentifier(Context);
        var user = await _userService.GetOrCreateUser(nameIdentifier);
        var hasAccess = await _clanService.UserHasAccessToClan(user.Id, clanId);
        if (!hasAccess)
        {
            // TODO: do not silently fail
            return;
        }
        
        await Groups.AddToGroupAsync(Context.ConnectionId, $"chat/{clanId.ToString()}");
    }

    public async Task StopClanChatListen(int clanId)
    {
        var nameIdentifier = GetNameIdentifier(Context);
        var user = await _userService.GetOrCreateUser(nameIdentifier);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat/{clanId.ToString()}");
    }

    private string GetNameIdentifier(HubCallerContext context)
    {
        return context.User!.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}