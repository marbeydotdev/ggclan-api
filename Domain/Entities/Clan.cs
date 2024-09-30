using System.ComponentModel.DataAnnotations;
using Domain.Common;

namespace Domain.Entities;

public class Clan : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool Private { get; set; } = false; // if true, won't show up in the clan browser
    public List<ClanMessage> Messages { get; set; } = [];
    public List<ClanMember> Members { get; set; } = [];
}