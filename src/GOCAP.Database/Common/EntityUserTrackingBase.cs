using GOCAP.Database.Common.Entities;

namespace GOCAP.Database.Common;

public abstract class EntityUserTrackingBase<TKey> : EntityBase<TKey>, IUserTracking
{
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
}
