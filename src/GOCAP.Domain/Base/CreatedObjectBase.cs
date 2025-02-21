namespace GOCAP.Domain;

public abstract class CreatedObjectBase : DateTrackingBase
{
    public string Name { get; set; } = string.Empty;
}
