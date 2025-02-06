namespace GOCAP.Admin.Api.AutoMapper;

public class AdminModelMapperProfile : ModelMapperProfileBase
{
    public AdminModelMapperProfile()
    {
        CreateMap<CountStatistics, CountStatisticsModel>().ReverseMap();
    }
}