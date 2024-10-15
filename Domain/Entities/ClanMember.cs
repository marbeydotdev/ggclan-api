using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class ClanMember : BaseEntity
{
    public User User { get; set; } = null!;
    public int UserId { get; set; }
    public Clan Clan { get; set; } = null!;
    public int ClanId { get; set; }
    public ClanMemberRole Role { get; set; }
}