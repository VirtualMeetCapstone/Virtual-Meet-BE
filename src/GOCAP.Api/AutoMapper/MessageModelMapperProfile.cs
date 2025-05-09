﻿namespace GOCAP.Api.AutoMapper;

public class MessageModelMapperProfile : ModelMapperProfileBase
{
    public MessageModelMapperProfile()
    {
        CreateMap<Message, MessageCreationModel>().ReverseMap();
        CreateMap<Message, MessageModel>().ReverseMap();
        CreateMap<MessageReaction, MessageReactionCreationModel>().ReverseMap();
        CreateMap<MessageReaction, MessageReactionModel>().ReverseMap();

        CreateMap<Conversation, ConversationModel>().ReverseMap();
        CreateMap<QueryResult<Conversation>, QueryResult<ConversationModel>>().ReverseMap();
    }
}
