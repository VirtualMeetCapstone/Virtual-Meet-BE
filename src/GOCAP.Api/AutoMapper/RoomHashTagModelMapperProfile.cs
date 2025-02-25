namespace GOCAP.Api.AutoMapper;

public class RoomHashTagModelMapperProfile : ModelMapperProfileBase
{
	public RoomHashTagModelMapperProfile()
	{
		CreateMap<RoomHashtag, RoomModel>().ReverseMap();
	}
}
