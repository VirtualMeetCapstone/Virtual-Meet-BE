namespace GOCAP.Repository.AutoMapper;

public class HashTagEntityMapperProfile : EntityMapperProfileBase
{
	public HashTagEntityMapperProfile()
	{
		CreateMap<Hashtag, HashTagEntity>().ReverseMap();
		CreateMap<RoomHashTag, RoomHashTagEntity>().ReverseMap();
	}
}
