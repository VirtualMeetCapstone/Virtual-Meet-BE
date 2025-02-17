using GOCAP.Database.Common.Entities;

namespace GOCAP.Database.Common;

public abstract class EntityUserTrackingNoIdBase : IUserTracking
{
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
}
