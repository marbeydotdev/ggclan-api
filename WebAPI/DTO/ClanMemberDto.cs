using Domain.Enums;

namespace WebAPI.DTO;

public class ClanMemberDto
{
    public int UserId { get; set; }
    public ClanMemberRole Role { get; set; }
}