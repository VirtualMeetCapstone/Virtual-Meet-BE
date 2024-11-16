namespace GOCAP.Api.Model;

public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Image { get; set; }
    public string? Bio { get; set; }
    public ICollection<Post>? Posts { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public ICollection<Follow>? Followers { get; set; }
    public ICollection<Follow>? Following { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
    public ICollection<UserRole>? UserRoles { get; set; }
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
}
