using GOCAP.Database;

namespace GOCAP.Domain;

public class HashTag : EntitySqlBase
{
	public string? Name { get; set; }
	public ICollection<RoomHashTag> Rooms { get; set; } = [];
}
