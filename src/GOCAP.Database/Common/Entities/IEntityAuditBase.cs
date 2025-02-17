namespace GOCAP.Database.Common.Entities;

public interface IEntityAuditBase<TKey> : IEntityBase<TKey>, IAuditable
{
}
