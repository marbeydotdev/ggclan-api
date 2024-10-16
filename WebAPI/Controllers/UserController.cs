using Application.DTO;
using Application.Users.Commands;
using Application.Users.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile = Domain.Entities.Profile;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1/user")]
public class UserController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public UserController(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMeAsync()
    {
        var user = await _mediator.Send(new GetUserCommand
        {
            NameIdentifier = HttpContext.GetNameIdentifier()
        });
        
        return Ok(_mapper.Map<UserDto>(user));
    }

    [Authorize]
    [HttpPost("update")]
    public async Task<IActionResult> UpdateUserAsync([FromBody] Profile profile)
    {
        var success = await _mediator.Send(new UpdateUserProfileCommand
        {
            NameIdentifier = HttpContext.GetNameIdentifier(),
            Profile = profile
        });

        if (success.IsFailed)
        {
            return BadRequest(success.Errors);
        }
        
        return Ok();
    }

    [Authorize]
    [HttpGet("friends")]
    public async Task<IActionResult> GetFriendsAsync()
    {
        var friends = await _mediator.Send(new GetFriendsQuery
        {
            NameIdentifier = HttpContext.GetNameIdentifier()
        });
        if (friends.IsFailed)
        {
            return BadRequest(friends.Errors);
        }
        
        return Ok(_mapper.Map<IEnumerable<UserDto>>(friends.Value));
    }
}