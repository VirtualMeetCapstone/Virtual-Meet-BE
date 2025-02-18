namespace GOCAP.Common;

public class QueryInfo
{
    public int Top { get; set; } = 10;
    public int Skip { get; set; } = 0;
    public string? SearchText { get; set; }
    public OrderType OrderType { get; set; } = OrderType.Descending;
    public string? OrderBy { get; set; } = AppConstants.DefaultOrderBy;
    public bool NeedTotalCount { get; set; } = true;
}
