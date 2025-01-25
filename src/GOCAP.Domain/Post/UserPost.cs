namespace GOCAP.Domain;

public class UserPost : DomainBase
{
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public IEnumerable<Media>? Medias { get; set; }
    public long CreateTime { get; set; } = DateTime.UtcNow.Ticks;
}
