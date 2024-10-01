using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO;

public class CreateClanDto
{
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    [MaxLength(5000)]
    public string Description { get; set; } = "";
    public bool Private { get; set; } = false; // if true, won't show up in the clan browser
}