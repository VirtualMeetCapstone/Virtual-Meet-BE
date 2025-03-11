namespace GOCAP.Api.Model;

public class PostCreationModel
{
    public required string Content { get; set; }
    public Guid UserId { get; set; }
    public PrivacyType? Privacy { get; set; }
    public List<IFormFile>? MediaUploads { get; set; }
}
    