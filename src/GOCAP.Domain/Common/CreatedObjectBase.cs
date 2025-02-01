namespace GOCAP.Domain;

public abstract class CreatedObjectBase : DateObjectBase
{
    public required string Name { get; set; }
}
