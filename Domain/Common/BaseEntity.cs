namespace Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}