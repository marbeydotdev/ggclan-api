using Application.DTO;
using AutoMapper;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1/members")]
public class MemberController(IMapper mapper, IClanRepository clanRepository) : ControllerBase
{
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMemberList(int id)
    {
        var members = await clanRepository.GetClanMembersAsync(id);
        
        return Ok(mapper.Map<IEnumerable<ClanMemberDto>>(members.Value));
    }
}