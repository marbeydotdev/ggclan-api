namespace WebAPI.DTO;

public class ClanMessageDto
{
    public int Id { get; set; }
    public string Message { get; set; } = null!;
    public DateTime Created { get; set; }
    public ClanMemberDto ClanMember { get; set; } = null!;
}