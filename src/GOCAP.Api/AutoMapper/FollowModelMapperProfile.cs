namespace GOCAP.Api.AutoMapper;

public class FollowModelMapperProfile : ModelMapperProfileBase
{
    public FollowModelMapperProfile()
    {
        CreateMap<Follow, FollowModel>().ReverseMap();
        CreateMap<Follow, FollowCreationModel>().ReverseMap();
    }
}
