namespace GOCAP.Api.Model;

public class PostModel
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public ICollection<Comment> Comments { get; set; } = [];
    //public ICollection<MediaModel> Medias { get; set; } = [];
    public long CreateTime { get; set; }
}
