namespace WebAPI.DTO;

public class UpdateClanDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool Private { get; set; } = false;
}