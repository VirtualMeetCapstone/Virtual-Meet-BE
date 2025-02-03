using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace GOCAP.Database;

public abstract class EntityMongoBase
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public required long CreateTime { get; set; } = 0;
    public required long LastModifyTime { get; set; } = 0;
}
