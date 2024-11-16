namespace GOCAP.Common;

public class QueryInfo
{
    public int Top { get; set; } = 10;
    public int Skip { get; set; } = 0;
    public string? Search { get; set; }
    public OrderType OrderType { get; set; } = OrderType.Ascending;
    public string? OrderBy { get; set; } = "CreateTime";
    public bool NeedTotalCount { get; set; } = true;
}
