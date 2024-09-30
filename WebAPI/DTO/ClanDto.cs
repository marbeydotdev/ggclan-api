using Domain.Entities;

namespace WebAPI.DTO;

public class ClanDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool Private { get; set; } = false; // if true, won't show up in the clan browser
    public List<ClanMemberDto> Members { get; set; } = [];
}