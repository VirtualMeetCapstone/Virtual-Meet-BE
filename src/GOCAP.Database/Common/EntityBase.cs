namespace GOCAP.Database;

public class EntityBase
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
}
