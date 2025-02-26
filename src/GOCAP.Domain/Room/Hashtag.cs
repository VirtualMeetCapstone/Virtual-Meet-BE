using GOCAP.Database;

namespace GOCAP.Domain;

public class Hashtag : EntitySqlBase
{
	public string? Name { get; set; }
	public ICollection<RoomHashtag> Rooms { get; set; } = [];
}
