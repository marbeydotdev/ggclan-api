using Domain.Common;

namespace Domain.Entities;

public class ClanMember : BaseEntity
{
    public User User { get; set; } = null!;
    public Clan Clan { get; set; } = null!;
}