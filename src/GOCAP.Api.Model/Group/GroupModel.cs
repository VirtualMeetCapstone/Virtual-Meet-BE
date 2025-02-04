namespace GOCAP.Api.Model;

public class GroupModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; } 
    public string? Picture { get; set; }
}