using GOCAP.Database.Common.Entities;

namespace GOCAP.Database.Common;

public abstract class EntityBase<TKey> : IEntityBase<TKey>
{
    public virtual TKey? Id { get; set; }
}
