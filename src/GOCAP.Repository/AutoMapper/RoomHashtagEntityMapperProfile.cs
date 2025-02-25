namespace GOCAP.Repository.AutoMapper;

public class RoomHashtagEntityMapperProfile : EntityMapperProfileBase
{
	public RoomHashtagEntityMapperProfile()
	{
		CreateMap<RoomHashtag, RoomHashTagEntity>().ReverseMap();
	}
}
