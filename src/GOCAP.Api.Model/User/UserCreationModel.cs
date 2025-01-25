namespace GOCAP.Api.Model;

public class UserCreationModel
{
    public string? Name { get; set; }
    public IFormFile? Picture { get; set; }
    public string? Bio { get; set; }
}
