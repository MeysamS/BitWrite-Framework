namespace Bw.Persistence.Dapper.SqlServer.Model;

public class PaginationResult<T>
{
    public int TotalRowCount { get; set; }
    public ICollection<T>? Rows { get; set; }
}
