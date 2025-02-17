using GOCAP.Database.Common.Entities;

namespace GOCAP.Database.Common;

public abstract class EntityDateTrackingNoIdBase : IDateTracking
{
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
}
