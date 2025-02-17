namespace GOCAP.Database.Common.Entities;

public interface IAuditable : IDateTracking, IUserTracking, ISoftDelete
{
}
