namespace GOCAP.Repository.AutoMapper;

public class PermissionEntityMapperProfile : EntityMapperProfileBase
{
    public PermissionEntityMapperProfile()
    {
        CreateMap<Permission, PermissionEntity>().ReverseMap();
    }
}