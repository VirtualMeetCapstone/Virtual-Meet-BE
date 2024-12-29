namespace GOCAP.Domain;

public abstract class DomainBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
}
