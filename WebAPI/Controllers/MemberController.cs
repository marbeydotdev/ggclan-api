using Application.DTO;
using Application.Users.Services;
using AutoMapper;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1/members")]
public class MemberController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IClanRepository _clanRepository;

    public MemberController(IMapper mapper, IUserService userService, IClanRepository clanRepository)
    {
        _mapper = mapper;
        _userService = userService;
        _clanRepository = clanRepository;
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMemberList(int id)
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());

        var members = await _clanRepository.GetClanMembersAsync(id);
        
        return Ok(_mapper.Map<IEnumerable<ClanMemberDto>>(members.Value));
    }
}