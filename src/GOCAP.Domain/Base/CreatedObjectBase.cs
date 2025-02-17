namespace GOCAP.Domain;

public abstract class CreatedObjectBase : DateTrackingBase
{
    public required string Name { get; set; }
}
