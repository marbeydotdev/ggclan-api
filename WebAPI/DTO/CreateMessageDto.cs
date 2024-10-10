using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO;

public class CreateMessageDto
{
    [MaxLength(500)]
    public string Message { get; set; } = null!;
}