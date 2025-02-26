namespace GOCAP.Repository.AutoMapper;

public class RoleEntityMapperProfile : EntityMapperProfileBase
{
    public RoleEntityMapperProfile()
    {
        CreateMap<UserRole, UserRoleEntity>().ReverseMap();
    }
}
