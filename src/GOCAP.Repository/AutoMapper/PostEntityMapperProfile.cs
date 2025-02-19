using System.Collections.Generic;

namespace GOCAP.Repository.AutoMapper;

public class PostEntityMapperProfile : EntityMapperProfileBase
{
    public PostEntityMapperProfile()
    {
        CreateMap<Post, PostEntity>()
           .ForMember(dest => dest.Medias, opt => opt
           .MapFrom(src => JsonHelper.Serialize(src.Medias)));

        CreateMap<PostEntity, Post>()
                 .ForMember(dest => dest.Medias, opt => opt
                 .MapFrom(src => JsonHelper.Deserialize<List<Media>>(src.Medias)));

        CreateMap<PostReaction, PostReactionEntity>().ReverseMap();
        CreateMap<QueryResult<Post>, QueryResult<PostEntity>>().ReverseMap();
    }
}