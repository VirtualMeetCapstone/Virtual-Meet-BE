namespace GOCAP.Api.AutoMapper;

public class RoomModelMapperProfile : ModelMapperProfileBase
{
    public RoomModelMapperProfile()
    {
        CreateMap<User, RoomMemberModel>().ReverseMap();
        CreateMap<Room, RoomModel>().ReverseMap();
        CreateMap<Room, RoomCreationModel>()
            .ForMember(dest => dest.MediaUploads, opt => opt.MapFrom(src => ConvertMediasToFormFiles(src.MediaUploads)));

        CreateMap<RoomCreationModel, Room>()
            .ForMember(dest => dest.MediaUploads, opt => opt.MapFrom(src => ConvertFormFilesToMedias(src.MediaUploads)));

        CreateMap<Room, RoomUpdationModel>()
            .ForMember(dest => dest.MediaUploads, opt => opt.MapFrom(src => ConvertMediasToFormFiles(src.MediaUploads)));

        CreateMap<RoomUpdationModel, Room>()
            .ForMember(dest => dest.MediaUploads, opt => opt.MapFrom(src => ConvertFormFilesToMedias(src.MediaUploads)));

        CreateMap<QueryResult<Room>, QueryResult<RoomModel>>().ReverseMap();

        // Mapper for room favourite
        CreateMap<RoomFavourite, RoomFavouriteModel>().ReverseMap();
        CreateMap<RoomFavourite, RoomFavouriteCreationModel>().ReverseMap();
        CreateMap<QueryResult<RoomFavourite>, QueryResult<RoomFavouriteModel>>().ReverseMap();
		CreateMap<Room, RoomFavouriteDetailModel>()
			.ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.Id))
			.ReverseMap();

		CreateMap<QueryResult<Room>, QueryResult<RoomFavouriteDetailModel>>()
			.ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data))
			.ReverseMap();
	}
}