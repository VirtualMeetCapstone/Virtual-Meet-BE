namespace GOCAP.Services.Intention;

public interface ICountStatisticService 
{
    Task<CountStatistics> GetStatisticsCountAsync();
}
