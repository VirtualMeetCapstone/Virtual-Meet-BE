namespace GOCAP.Domain;

public class LogoUpdate : DateTrackingBase
{
    public required MediaUpload Media { get; set; }
}
