namespace GOCAP.Domain;

public class GroupDetail : CreatedObjectBase
{
    public Guid OwnerId { get; set; }
    public User? Owner { get; set; }
    public string? Picture { get; set; }
    public ICollection<User> Members { get; set; } = [];
}
