namespace GOCAP.Api.Model;
public class CommentAuthorModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Picture { get; set; }
}