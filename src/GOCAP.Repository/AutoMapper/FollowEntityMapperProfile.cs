namespace GOCAP.Repository.AutoMapper;

public class FollowEntityMapperProfile : EntityMapperProfileBase
{
    public FollowEntityMapperProfile()
    {
        CreateMap<Follow, UserFollowEntity>().ReverseMap();
    }
}
