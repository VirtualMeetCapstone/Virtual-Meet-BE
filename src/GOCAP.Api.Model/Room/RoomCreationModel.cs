namespace GOCAP.Api.Model;

public class RoomCreationModel
{
    public Guid OwnerId { get; set; }
    public required string Topic { get; set; }
    public string? Description { get; set; }
    public int MaximumMembers { get; set; }
    public List<IFormFile>? MediaUploads { get; set; }
}
