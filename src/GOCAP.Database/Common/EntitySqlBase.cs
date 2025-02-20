using GOCAP.Database.Common;

namespace GOCAP.Database;

public abstract class EntitySqlBase : EntityDateTrackingBase<Guid>
{
    [Key]
    public override Guid Id { get; set; }
}
