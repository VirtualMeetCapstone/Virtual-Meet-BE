namespace GOCAP.Domain;

public class Logo : DateTrackingBase
{
    public Media? Media { get; set; }
    public required MediaUpload MediaUpload { get; set; }
}
