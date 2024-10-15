using Domain.Enums;

namespace WebAPI.DTO;

public class ClanMemberDto
{
    public DateTime Created { get; set; }
    public UserDto User { get; set; } = null!;
    public ClanMemberRole Role { get; set; }
}