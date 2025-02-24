namespace GOCAP.Api.AutoMapper;

public class SearchModelMapperProfile : ModelMapperProfileBase
{
    public SearchModelMapperProfile()
    {
        CreateMap<SearchHistory, SearchHistoryModel>().ReverseMap();
        CreateMap<SearchHistory, SearchHistoryCreationModel>().ReverseMap();
        CreateMap<QueryResult<SearchHistory>, QueryResult<SearchHistoryModel>>().ReverseMap();
    }
}
