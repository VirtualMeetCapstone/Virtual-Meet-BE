namespace GOCAP.Database;

public abstract class EntitySqlBase
{
    [Key]
    public Guid Id { get; set; }
}
