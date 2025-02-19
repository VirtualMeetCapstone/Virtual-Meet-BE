namespace GOCAP.Common;

public enum PermissionType
{
    // Guest
    ViewRooms = 0,
    ViewPosts = 1,

    // User
    JoinRoom = 2,
    CommentPost = 3,
    ReactToPost = 4,
    ReportPost = 5,

    // Paticipant
    SendMessage = 6,
    EditMessage = 7,
    DeleteMessage = 8,
    ReplyToMessage = 9,
    ReactToMessage = 10,
    ViewMembers = 11,

    // Room owner
    ManageRoomSettings = 12,
    ManageRoomRoles = 13,
    DeleteRoom = 14,
    RemoveMember = 15,
    MuteMember = 16,
    PinMessage = 17,

    // Admin
    ManageUsers = 18,
    ManageRooms = 19,
    ManageReports = 20,
    ManagePermissions = 21,
    BanUser = 22,
    UnbanUser = 23,

    // System
}
