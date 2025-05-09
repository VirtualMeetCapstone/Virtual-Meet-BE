﻿using Microsoft.EntityFrameworkCore;

namespace GOCAP.Services;

[RegisterService(typeof(INotificationService))]
internal class NotificationService(
    AppSqlDbContext _dbContext,
    INotificationRepository _repository,
    IStoryRepository _storyRepository,
    IUserRepository _userRepository,
    IFollowRepository _followRepository,
    IPostRepository _postRepository,
    IEmailService _emailService,
    IMapper _mapper,
    ILogger<NotificationService> _logger
    ) : ServiceBase<Notification, NotificationEntity>(_repository, _mapper, _logger), INotificationService
{
    private readonly IMapper _mapper = _mapper;
    public async Task<Notification> HandleNotificationEvent(NotificationEvent notificationEvent)
    {
        if (notificationEvent.Type == NotificationType.System)
        {
            var users = await _userRepository.GetAllAsync();

            string subject = "[No Subject]";
            string content = "[No Content]";
            string roomId = "";

            if (notificationEvent.Metadata != null)
            {
                if (notificationEvent.Metadata.TryGetValue("Subject", out var s))
                    subject = s;

                if (notificationEvent.Metadata.TryGetValue("Content", out var c))
                    content = c;

                if (notificationEvent.Metadata.TryGetValue("RoomId", out var r))
                {
                    roomId = r;
                    var roomIdGuid = Guid.Parse(roomId);

                    var roomMemberIds = await _dbContext.RoomMembers
                        .Where(rm => rm.RoomId == roomIdGuid)
                        .Select(rm => rm.UserId)
                        .ToListAsync();


                    var sendEmailToMemberTasks = users
                            .Where(u => roomMemberIds.Contains(u.Id) && !string.IsNullOrWhiteSpace(u.Email))
                            .Select(user =>
                                _emailService.SendMailAsync(new EmailContent
                                {
                                    To = user.Email,
                                    Subject = subject,
                                    Body = content
                                })
                            );

                    await Task.WhenAll(sendEmailToMemberTasks);

                    return null!;
                }
                    
            }

            var sendEmailTasks = users
                    .Where(u => !string.IsNullOrWhiteSpace(u.Email))
                    .Select(user =>
                        _emailService.SendMailAsync(new EmailContent
                        {
                            To = user.Email,
                            Subject = subject,
                            Body = content
                        })
                    );

            await Task.WhenAll(sendEmailTasks);

            return null!;
        }

        var actor = await _userRepository.GetByIdAsync(notificationEvent.ActorId);
        var notification = new Notification
        {
            Type = notificationEvent.Type,
            ActionType = notificationEvent.ActionType,
            Actor = new NotificationActor
            {
                Id = actor.Id,
                Name = actor.Name,
                Picture = JsonHelper.Deserialize<Media>(actor.Picture),
            },
            Source = notificationEvent.Source
        };
        var notificationMessage = new NotificationMessageBuilder()
                                                        .SetAction(GetAction(notificationEvent.Type))
                                                        .SetTarget(GetTarget(notificationEvent.Type, notificationEvent.Source))
                                                        .Build();
        notification.Content = notificationMessage;
        notification.InitCreation();
        switch (notificationEvent.Type)
        {
            case NotificationType.Story:
            case NotificationType.Post:
            case NotificationType.Room:
                notification.UserIds = await _followRepository.GetFollowersByUserIdAsync(notificationEvent.ActorId);
                break;
            case NotificationType.Follow:
                notification.UserIds = [notificationEvent.UserId];
                break;
            case NotificationType.Comment:
                var post = await _postRepository.GetByIdAsync(notificationEvent.Source?.Id ?? throw new InternalException());
                notification.UserIds = [post.UserId];
                break;
            case NotificationType.Reaction:
                switch (notificationEvent.Source?.Type)
                {
                    case SourceType.Story:
                        notification.UserIds = [await _storyRepository.GetUserIdByStoryIdAsync(notificationEvent.Source.Id)];
                        break;
                    case SourceType.Room:
                        break;
                    case SourceType.Post:
                        break;
                    case SourceType.Comment:
                        break;
                }
                break;
            default: throw new ParameterInvalidException();
        }
        var entity = _mapper.Map<NotificationEntity>(notification);
        var result = await _repository.AddAsync(entity);
        return _mapper.Map<Notification>(result);
    }

    private static string GetAction(NotificationType type)
    {
        return type switch
        {
            NotificationType.Reaction => "reacted on",
            NotificationType.Story => "posted",
            NotificationType.Post => "created",
            NotificationType.Room => "created",
            NotificationType.Follow => "followed",
            NotificationType.Comment => "commented on",
            _ => "performed an action"
        };
    }

    private static string GetTarget(NotificationType type, NotificationSource? source)
    {
        return type switch
        {
            NotificationType.Story => "a new story",
            NotificationType.Post => "a new post",
            NotificationType.Room => "a new room",
            NotificationType.Follow => "you",
            NotificationType.Comment => "your post",
            NotificationType.Reaction => GetReactionTarget(source),
            _ => "something"
        };
    }

    private static string GetReactionTarget(NotificationSource? source)
    {
        return source?.Type switch
        {
            SourceType.Post => "your post",
            SourceType.Comment => "your comment",
            SourceType.Story => "your story",
            _ => "your content"
        };
    }

    public async Task<QueryResult<Notification>> GetNotificationsByUserIdAsync(Guid userId, QueryInfo queryInfo)
    => await _repository.GetNotificationsByUserIdAsync(userId, queryInfo);

    public async Task<OperationResult> MarkAsReadAsync(Guid userId, Guid notificationId)
    {
        _logger.LogInformation("Start marking a notification as read.");
        return new OperationResult(await _repository.MarkAsReadAsync(userId, notificationId));
    }
}