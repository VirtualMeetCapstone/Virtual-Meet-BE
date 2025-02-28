using GOCAP.Database.Common;

namespace GOCAP.Database;

public abstract class EntityMongoBase : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
}
