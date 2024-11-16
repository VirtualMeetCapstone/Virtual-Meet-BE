using System.ComponentModel.DataAnnotations.Schema;

namespace GOCAP.Database;

[Table("Follows")]
public class FollowEntity : DateObjectCommon
{
    [Required]
    public Guid FollowerId { get; set; }

    [Required]
    public Guid FollowingId { get; set; }

    // Relationships
    public UserEntity? Follower { get; set; }
    public UserEntity? Following { get; set; }
}
