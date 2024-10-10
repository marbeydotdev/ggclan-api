using System.Security.Claims;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTO;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1/user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly GGDbContext _context;
    private readonly IMapper _mapper;

    public UserController(UserService userService, UserRepository userRepository, IMapper mapper, GGDbContext context)
    {
        _userService = userService;
        _userRepository = userRepository;
        _mapper = mapper;
        _context = context;
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

    [Authorize]
    [HttpGet("friends")]
    public async Task<IActionResult> GetFriendsAsync()
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());
        var userClans = _context.Clans
            .Where(clan => clan.Members.Any(member => member.UserId == user.Id));

        var friends = await userClans
            .SelectMany(clan => clan.Members)
            .Where(member => member.UserId != user.Id)
            .Select(member => member.User)
            .Distinct()
            .ToListAsync();
        
        return Ok(_mapper.Map<IEnumerable<UserDto>>(friends));
    }
}