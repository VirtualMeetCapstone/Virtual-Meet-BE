namespace GOCAP.Database.Common.Entities;

public interface IDateTracking
{
    long CreateTime { get; set; }   
    long LastModifyTime { get; set; }
}
