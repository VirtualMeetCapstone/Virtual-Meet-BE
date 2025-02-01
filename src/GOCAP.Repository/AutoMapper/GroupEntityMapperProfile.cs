namespace GOCAP.Repository.AutoMapper;

public class GroupEntityMapperProfile : EntityMapperProfileBase
{
    public GroupEntityMapperProfile()
    {
        CreateMap<Group, GroupEntity>().ReverseMap();
        CreateMap<GroupMember, GroupMemberEntity>().ReverseMap();
    }
}
