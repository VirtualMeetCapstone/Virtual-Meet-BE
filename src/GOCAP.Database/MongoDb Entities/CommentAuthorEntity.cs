namespace GOCAP.Database;
public class CommentAuthorEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Picture { get; set; }
}
