namespace GOCAP.Api.Model;

public class RoomFavouriteDetailModel
{
	public Guid Id { get; set; }
	public Guid? OwnerId { get; set; }
	public RoomMemberModel? Owner { get; set; }
	public string Topic { get; set; } = string.Empty;
	public List<Media>? Medias { get; set; } = null;
}
