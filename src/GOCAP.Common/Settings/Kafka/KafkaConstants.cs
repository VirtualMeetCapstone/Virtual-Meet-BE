namespace GOCAP.Common;
public static class KafkaConstants
{
    public const string GroupId = "gocap-consumers";
    public static class Topics
    {
        public const string UserLogin = "user-login-events";
        public const string Notification = "notification-events";
        public const string SearchHistory = "search-history-events";
    }
}
