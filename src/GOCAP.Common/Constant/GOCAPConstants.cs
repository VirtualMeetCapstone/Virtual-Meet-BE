namespace GOCAP.Common;

public static class GOCAPConstants
{
    public static readonly string DatabaseName = "GOCAP";
    public static readonly string DefaultUri = "https://localhost:7035";
    public static readonly string DefaultGroupPicture = "";
    public static readonly string DefaultUserPicture = "";
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;
    public const int DefaultPage = 1;
    public const string DefaultOrderBy = "CreateTime";
    public const string DefaultOrderType = "DESC";
    public const string SqlServerConnection = "SqlServerConnection";
    public const string MongoDbConnection = "MongoDbConnection";
    public const string RedisConnection = "RedisConnection";
    public const string AzureBlobStorage = "AzureBlobStorage";
    public const int DefaultMinLength = 1;

    // Default max length 
    public const int MaxLengthTopic = 100;
    public const int MaxLengthName = 30;
    public const int MaxLengthEmail = 320;
    public const int MaxLengthDescription = 1000;
    public const int MaxLengthHastTag = 50;
    public const int MaxLengthUrl = 2000;

    // Default media file size
    public const long MaxBlobSize = 1024 * 1024 * 50;
    public const string BlobContainer = "gocap";
}
