using Domain.Common;

namespace Application.DTO;

public class ClanMessageDto : BaseEntity
{
    public string Message { get; set; } = null!;
    public ClanMemberDto ClanMember { get; set; } = null!;
}