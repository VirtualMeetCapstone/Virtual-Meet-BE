using System.Xml;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

public class MessagesOutsideRoomEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Content { get; set; }
    public UserInfo Sender { get; set; }
    public string ConversationId { get; set; }
    public string ConversationType { get; set; } 

    public UserInfo? Receiver { get; set; }

    // Fields for group chat
    public string? GroupId { get; set; }  
    public string? GroupName { get; set; }  
    public List<UserInfo>? GroupMembers { get; set; } = new();

    // Message metadata
    public string? ReplyToMessageId { get; set; }  // Nullable string
    public MessagesOutsideRoomEntity? ReplyMessage { get; set; }  // Nullable Message entity
    public List<ReactionInfo> Reactions { get; set; } = new();
    public string MessageType { get; set; } = "text";
    public List<string> AttachmentUrls { get; set; } = new();

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }  // Nullable DateTime
    public DateTime? EditedAt { get; set; }   // Nullable DateTime

    // Read status
    public List<ReadStatus>? ReadStatuses { get; set; } = new();
    public bool IsDeleted { get; set; } = false;
}

public enum ConversationType
{
    Private,    /
    Group      
}

public class UserInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? ImageUrl { get; set; } 
}

public class ReactionInfo
{
    public string UserId { get; set; }
    public string Emoji { get; set; }
}

public class ReadStatus
{
    public string UserId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }  
}
