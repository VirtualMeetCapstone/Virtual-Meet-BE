namespace GOCAP.Database;

[Table("PostTags")]
public class PostTagEntity : EntitySqlBase
{
    public Guid PostId { get; set; }
    public Guid TaggedUserId { get; set; }

    public PostEntity? Post { get; set; } 
    public UserEntity? TaggedUser { get; set; } 

}

