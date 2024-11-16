namespace GOCAP.Api.Model;

public class PostCreationModel
{
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public ICollection<IFormFile>? MediaFiles { get; set; }
}
