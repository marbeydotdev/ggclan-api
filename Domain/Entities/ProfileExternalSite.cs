using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class ProfileExternalSite
{
    [MaxLength(50)]
    public string Name { get; set; } = null!;
    [MaxLength(500)]
    public string Url { get; set; } = null!;
}