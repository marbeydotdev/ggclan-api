using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class CreateMessageDto
{
    [MaxLength(500)]
    [MinLength(1)]
    public string Message { get; set; } = null!;
}