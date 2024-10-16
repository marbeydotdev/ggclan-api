using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class UpdateClanDto
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    [MaxLength(5000)]
    public string Description { get; set; } = null!;
    public bool Private { get; set; } = false;
}