namespace GOCAP.Api.AutoMapper;

public class HashTagModelMapperProfile : ModelMapperProfileBase
{
	public HashTagModelMapperProfile()
	{
		CreateMap<HashTag, HashTagModel>().ReverseMap();
		CreateMap<RoomHashTag, RoomModel>().ReverseMap();
	}
}
