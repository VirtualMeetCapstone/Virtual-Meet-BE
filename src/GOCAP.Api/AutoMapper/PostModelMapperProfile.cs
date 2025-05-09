﻿namespace GOCAP.Api.AutoMapper;

public class PostModelMapperProfile : ModelMapperProfileBase
{
    public PostModelMapperProfile()
    {
        CreateMap<QueryResult<Post>, QueryResult<PostModel>>().ReverseMap();
        CreateMap<Post, PostModel>().ReverseMap();
        CreateMap<User, UserPostModel>().ReverseMap();

        CreateMap<Post, PostCreationModel>()
            .ForMember(dest => dest.MediaUploads, opt => opt
            .MapFrom(src => ConvertMediaHelper.ConvertMediasToFormFiles(src.MediaUploads)));

        CreateMap<PostCreationModel, Post>()
            .ForMember(dest => dest.MediaUploads, opt => opt
            .MapFrom(src => ConvertMediaHelper.ConvertFormFilesToMedias(src.MediaUploads)));


        CreateMap<PostReaction, PostReactionCreationModel>().ReverseMap();
        CreateMap<UserReactionPost, UserReactionPostModel>().ReverseMap();

    }
}
