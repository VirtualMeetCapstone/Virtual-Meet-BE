using GOCAP.Database.Common.Entities;

namespace GOCAP.Domain;

public abstract class UserTrackingBase : DomainBase, IUserTracking
{
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }

    public void InitCreation()
    {
        Id = Guid.NewGuid();
    }
}
