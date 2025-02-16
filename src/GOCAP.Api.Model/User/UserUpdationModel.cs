namespace GOCAP.Api.Model;

public class UserUpdationModel
{
    public string? Name { get; set; }
    public IFormFile? PictureUpload { get; set; }
    public string? Bio { get; set; }
}
