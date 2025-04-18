namespace GOCAP.Domain;

public class UserReport
{
    public int TotalUsers { get; set; }
    public int NewUsers { get; set; }
    public int BannedUsers { get; set; }
    public double RetentionRate { get; set; }
    public double GrowthRate { get; set; }
}
