namespace GOCAP.Repository.AutoMapper;

public class HashTagEntityMapperProfile : EntityMapperProfileBase
{
	public HashTagEntityMapperProfile()
	{
		CreateMap<HashTag, HashTagEntity>().ReverseMap();
		CreateMap<RoomHashTag, RoomHashTagEntity>().ReverseMap();
	}
}
