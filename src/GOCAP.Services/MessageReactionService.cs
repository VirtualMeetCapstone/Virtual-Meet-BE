﻿namespace GOCAP.Services;

[RegisterService(typeof(IMessageReactionService))]
internal class MessageReactionService(
    IMessageReactionRepository _repository,
    IMapper _mapper,
    ILogger<MessageReactionService> _logger
    ) : ServiceBase<MessageReaction, MessageReactionEntity>(_repository, _mapper, _logger), IMessageReactionService
{
}