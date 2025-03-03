namespace GOCAP.Messaging;

public static class KafkaConstants
{
    public const string GroupId = "gocap-consumers";
    public static class Topics
    {
        public const string UserLogin = "user-login-events";
    }
}
