namespace GOCAP.Api.Model;

public class GroupCreationModel
{
    public required string Name { get; set; }
    public Guid OwnerId { get; set; }
    public IFormFile? Picture { get; set; }
}
