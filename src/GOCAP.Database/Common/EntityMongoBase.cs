using GOCAP.Database.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace GOCAP.Database;

public abstract class EntityMongoBase: EntityDateTrackingBase<Guid>
{
    [BsonId]
    public override Guid Id { get; set; }
}
