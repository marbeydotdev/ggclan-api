using Domain.Common;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string NameIdentifier  { get; set; } = null!;
    public Profile Profile { get; set; } = new Profile();
    public List<ClanMember> Clans { get; set; } = [];
}