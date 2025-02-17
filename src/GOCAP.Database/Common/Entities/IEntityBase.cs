namespace GOCAP.Database.Common.Entities;

public interface IEntityBase<TKey>
{
    TKey? Id { get; set; }
}
