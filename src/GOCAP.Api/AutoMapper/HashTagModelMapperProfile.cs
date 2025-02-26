namespace GOCAP.Api.AutoMapper;

public class HashTagModelMapperProfile : ModelMapperProfileBase
{
	public HashTagModelMapperProfile()
	{
		CreateMap<Hashtag, HashtagModel>().ReverseMap();
		CreateMap<RoomHashTag, RoomModel>().ReverseMap();
	}
}
