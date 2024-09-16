using Domain.Common;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;
    public List<ClanMember> ParticipatesIn { get; set; } = [];
}