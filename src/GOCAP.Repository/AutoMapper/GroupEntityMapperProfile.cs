namespace GOCAP.Repository.AutoMapper;

public class GroupEntityMapperProfile : EntityMapperProfileBase
{
    public GroupEntityMapperProfile()
    {
        CreateMap<Group, GroupEntity>().ReverseMap();
        CreateMap<QueryResult<Group>, QueryResult<GroupEntity>>().ReverseMap();
        CreateMap<GroupMember, GroupMemberEntity>().ReverseMap();
    }
}
