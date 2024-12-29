namespace GOCAP.Domain;

public abstract class DateObjectBase : DomainBase
{
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }

    public void InitCreation()
    {
        this.Id = Guid.NewGuid();
        this.CreateTime = DateTime.UtcNow.Ticks;
        this.LastModifyTime = DateTime.UtcNow.Ticks;
    }

    public void UpdateModify()
    {
        this.LastModifyTime = DateTime.UtcNow.Ticks;
    }
}
