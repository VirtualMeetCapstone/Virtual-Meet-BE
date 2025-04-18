namespace GOCAP.Api.Model;
public class LogoModel
{
    public Media? Media { get; set; }
    public required IFormFile MediaUpload { get; set; }
}
