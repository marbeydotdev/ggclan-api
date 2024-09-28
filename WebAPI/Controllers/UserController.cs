using System.Security.Claims;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1/user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserController(UserService userService, UserRepository userRepository, IMapper mapper)
    {
        _userService = userService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMeAsync()
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());
        return Ok(_mapper.Map<UserDto>(user));
    }

    [Authorize]
    [HttpPost("update")]
    public async Task<IActionResult> UpdateUserAsync([FromBody] ProfileDto profile)
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());
        
        user.Profile = _mapper.Map(profile, user.Profile);
        var success = await _userRepository.UpdateProfileAsync(user);

        if (success.IsFailed)
        {
            return BadRequest(success.Errors);
        }
        
        return Ok();
    }
}