namespace GOCAP.Domain;

public class NotificationActor
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Media? Picture { get; set; } 
}

