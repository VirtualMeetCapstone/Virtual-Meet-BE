namespace GOCAP.Database;

public class UserCreatedObject : EntityBase
{
    [Required]
    [StringLength(100)]
    public string? Name { get; set; }
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
