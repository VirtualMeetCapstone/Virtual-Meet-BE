namespace GOCAP.Database.Common.Entities;

public interface ISoftDelete
{
    public bool IsDeleted { get; set; }
    public long? DeleteTime {  get; set; }

    public void Undo()
    {
        IsDeleted = false;
        DeleteTime = null;
    }
}
