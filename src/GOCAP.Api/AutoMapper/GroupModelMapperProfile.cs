namespace GOCAP.Api.AutoMapper;

public class GroupModelMapperProfile : ModelMapperProfileBase
{
    public GroupModelMapperProfile()
    {
        CreateMap<Group, GroupModel>().ReverseMap();
        CreateMap<Group, GroupCreationModel>().ReverseMap();
        CreateMap<Group, GroupUpdationModel>().ReverseMap();
        CreateMap<QueryResult<Group>, QueryResult<GroupModel>>().ReverseMap();

        CreateMap<GroupMember, GroupMemberModel>().ReverseMap();
        CreateMap<GroupMember, GroupMemberCreationModel>().ReverseMap();


    }
}
