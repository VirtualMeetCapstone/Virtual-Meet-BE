﻿using GOCAP.Database;

namespace GOCAP.Api.AutoMapper;

public class UserModelMapperProfile : ModelMapperProfileBase
{
    public UserModelMapperProfile()
    {
        CreateMap<RefreshToken, RefreshTokenModel>().ReverseMap();
        CreateMap<User, UserModel>().ReverseMap();
        CreateMap<User, UserProfileModel>().ReverseMap();

        CreateMap<User, UserUpdationModel>()
            .ForMember(dest => dest.PictureUpload, opt => opt.MapFrom(src => ConvertMediaHelper.ConvertMediaToFormFile(src.PictureUpload)));

        CreateMap<UserUpdationModel, User>()
            .ForMember(dest => dest.PictureUpload, opt => opt.MapFrom(src => ConvertMediaHelper.ConvertFormFileToMedia(src.PictureUpload)));

        CreateMap<QueryResult<User>, QueryResult<UserModel>>().ReverseMap();

        CreateMap<UserBlockCreationModel, UserBlock>().ReverseMap();
        CreateMap<UserBlock, UserBlockModel>().ReverseMap();
        CreateMap<User, UserSearchModel>().ReverseMap();
        CreateMap<UserVip, UserVipModel>();
    }
}
