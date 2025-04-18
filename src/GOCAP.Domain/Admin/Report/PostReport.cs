namespace GOCAP.Domain;

public class PostReport
{
    public int TotalPosts { get; set; }
    public int NewPosts { get; set; }
    public int DeletedPosts { get; set; }
    public double GrowthRate { get; set; }
}
