namespace GOCAP.Database;

public class EntitySqlBase
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
}
