using Domain.Common;

namespace Domain.Entities;

public class ClanMessage : BaseEntity
{
    public Clan Clan { get; set; } = null!;
    public int ClanId { get; set; }
    public ClanMember ClanMember { get; set; } = null!;
    public int ClanMemberId { get; set; }
    public string Message { get; set; } = null!;
}