namespace Domain.Entities;

public class ClanWithMember
{
    public Clan Clan { get; set; } = null!;
    public List<ClanMember> Members { get; set; } = null!;
}