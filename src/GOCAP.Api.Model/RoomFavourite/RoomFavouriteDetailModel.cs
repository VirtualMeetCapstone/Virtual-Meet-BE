namespace GOCAP.Api.Model.RoomFavourite;

public class RoomFavouriteDetailModel
{
	public Guid RoomId { get; set; }
	public Guid? OwnerId { get; set; }
	public RoomMemberModel? Owner { get; set; }
	public string Topic { get; set; } = string.Empty;
	public List<Media>? Medias { get; set; } = null;
}
