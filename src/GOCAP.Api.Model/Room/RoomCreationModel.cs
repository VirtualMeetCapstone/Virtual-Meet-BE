namespace GOCAP.Api.Model;

public class RoomCreationModel
{
    public Guid OwnerId { get; set; }
    public string? Topic { get; set; }
    public string? Description { get; set; }
    public int MaximumMembers { get; set; }
    public ICollection<IFormFile>? Medias { get; set; }
}
