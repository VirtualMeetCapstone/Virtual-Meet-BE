namespace GOCAP.Api.Model;

public class PostReportModel
{
    public int TotalPosts { get; set; }
    public int NewPosts { get; set; }
    public int DeletedPosts { get; set; }
    public double GrowthRate { get; set; }
}
