namespace GOCAP.Domain;

public class FacebookUser : ProviderUserBase
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Birthday { get; set; } = string.Empty;
    public string Hometown { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<FacebookFriendModel> Friends { get; set; } = [];
    public PictureModel? Picture { get; set; }
}

public class PictureModel
{
    public DataModel? Data { get; set; }
}

public class DataModel
{
    public int Height { get; set; }
    public bool IsSilhouette { get; set; }
    public string Url { get; set; } = string.Empty;
    public int Width { get; set; }
}

public class FacebookFriendModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
