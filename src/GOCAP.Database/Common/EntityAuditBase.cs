using GOCAP.Database.Common.Entities;

namespace GOCAP.Database.Common;

public abstract class EntityAuditBase<TKey> : EntityBase<TKey>, IEntityAuditBase<TKey>
{
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public long? DeleteTime { get; set; }
}
