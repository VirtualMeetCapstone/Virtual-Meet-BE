using System.ComponentModel.DataAnnotations.Schema;

namespace GOCAP.Database;

[Table("Comments")]
public class CommentEntity : DateObjectCommon
{
    [Required]
    public string? Content { get; set; }
    public MediaEntity? Media { get; set; }
    public int? LikeCount { get; set; }
    public Guid? ParentId { get; set; }
    [Required]
    public Guid PostId { get; set; }
    [Required]
    public Guid UserId { get; set; }

}
