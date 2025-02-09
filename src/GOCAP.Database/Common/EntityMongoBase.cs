using MongoDB.Bson.Serialization.Attributes;
namespace GOCAP.Database;

public abstract class EntityMongoBase
{
    [BsonId]
    public Guid Id { get; set; }
    public required long CreateTime { get; set; } = 0;
    public required long LastModifyTime { get; set; } = 0;
}
