namespace GOCAP.Domain;

public abstract class DateObjectBase : DomainBase
{
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }

    public void InitCreation()
    {
        Id = Guid.NewGuid();
        CreateTime = DateTime.UtcNow.Ticks;
        LastModifyTime = DateTime.UtcNow.Ticks;
    }

    public void UpdateModify()
    {
        LastModifyTime = DateTime.UtcNow.Ticks;
    }
}
