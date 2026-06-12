namespace ChatApp.Application.DTOs.Common;

public class PagedResult<T>
{
    public IReadOnlyCollection<T> Items { get; set; }
        = new List<T>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }
}