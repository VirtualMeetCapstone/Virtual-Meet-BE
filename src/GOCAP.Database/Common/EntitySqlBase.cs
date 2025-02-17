using GOCAP.Database.Common;

namespace GOCAP.Database;

public abstract class EntitySqlBase : EntityDateTrackingBase<Guid>
{
    [Key]
    public required override Guid Id { get; set; }
}
