namespace GOCAP.Database.Common.Entities;

public interface IUserTracking
{
    Guid CreatedBy { get; set; }
    Guid? ModifiedBy { get; set; }
}
