namespace GOCAP.Common;

/// <summary>
/// Defines the type of notification the user receives.
/// </summary>
public enum NotificationType
{
    Reaction = 0,       // Someone reacted to a post, comment, or story.
    Follow = 1,         // Someone followed the user.
    Comment = 2,        // Someone commented on a post or story.
    Story = 3,          // Someone posted a new story.
    Room = 4,           // A room-related event (invite, mention, etc.).
    Post = 5,           // A new post was created or shared.
    Mention = 6,        // The user was mentioned in a post, comment, or story.
    Message = 7,        // The user received a private message.
    Group = 8,          // The user was added to a group or group event.
    Share = 9,          // Someone shared the user's post or content.
    Request = 10,       // A request-related notification (e.g., friend request, room join request).
    System = 11,        // A system notification (maintenance, updates, warnings).
    Achievement = 12,   // The user unlocked an achievement.
    Reminder = 13,      // A reminder for an event or scheduled activity.
    Event = 14,         // A notification about an upcoming or ongoing event.
    Report = 15,        // A report-related notification (e.g., content reported or reviewed).
    Admin = 16          // An admin-related notification (bans, warnings, etc.).
}

public enum SourceType
{
    Post = 0,       // The notification originates from a post.
    Comment = 1,    // The notification originates from a comment.
    Story = 2,      // The notification originates from a story.
    Room = 3,       // The notification originates from a room (e.g., someone added you to a room).
    User = 4,       // The notification originates from a user action (e.g., follow request).
    Message = 5,    // The notification originates from a private message.
    Group = 6,      // The notification originates from a group activity.
    System = 7      // The notification originates from a system event (e.g., maintenance or updates).
}