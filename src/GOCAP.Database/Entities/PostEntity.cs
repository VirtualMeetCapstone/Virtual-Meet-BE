using System.ComponentModel.DataAnnotations.Schema;

namespace GOCAP.Database;

[Table("Posts")]
public class PostEntity : EntityBase
{
    [Required]
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    public IEnumerable<MediaEntity>? Medias { get; set; }
    public long CreateTime { get; set; } = DateTime.UtcNow.Ticks;
}
