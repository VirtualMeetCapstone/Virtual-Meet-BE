namespace GOCAP.Common;

public class QueryResult<TDomain>
{
    /// <summary>
    /// The real data set
    /// </summary>
    public IEnumerable<TDomain> Data { get; set; } = [];

    /// <summary>
    /// The total count of the data set
    /// </summary>
    public int TotalCount { get; set; }
}
