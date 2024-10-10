using Domain.Entities;

namespace WebAPI.DTO;

public class ClanInviteDto
{
    public int Id { get; set; }
    public UserDto User { get; set; } = null!;
    public string Message { get; set; } = "";
}