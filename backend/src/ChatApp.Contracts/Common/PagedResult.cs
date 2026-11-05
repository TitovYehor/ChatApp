namespace ChatApp.Contracts.Common;

/// <summary>
/// Represents a page of results together with pagination metadata
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyCollection<T> Items { get; set; }
        = new List<T>();
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of matching items across all pages
    /// </summary>
    public int TotalCount { get; set; }
}