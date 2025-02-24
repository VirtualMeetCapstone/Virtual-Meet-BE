namespace GOCAP.Repository.AutoMapper;

public class SearchEntityMapperProfile : EntityMapperProfileBase
{
    public SearchEntityMapperProfile()
    {
        CreateMap<SearchHistory, SearchHistoryEntity>().ReverseMap();
        CreateMap<QueryResult<SearchHistory>, QueryResult<SearchHistoryEntity>>().ReverseMap();
    }
}
