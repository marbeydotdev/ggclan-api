using Domain.Common;

namespace Domain.Entities;

public class ClanInvite : BaseEntity
{
    public User User { get; set; } = null!;
    public int UserId { get; set; }
    public string Message { get; set; } = "";
}