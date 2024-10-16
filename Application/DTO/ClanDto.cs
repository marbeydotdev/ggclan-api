using System.ComponentModel.DataAnnotations;
using Domain.Common;

namespace Application.DTO;

public class ClanDto : BaseEntity
{
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public string Game { get; set; } = null!;
    [Required]
    public bool Private { get; set; } = false; // if true, won't show up in the clan browser
}