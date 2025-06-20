using System.Collections.Generic;

namespace Shared.Contracts.Common;

/// <summary>
/// Generic paginated response format.
/// </summary>
public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public PaginatedResult() {}

    public PaginatedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static PaginatedResult<T> Create(List<T> items, int totalCount, int pageNumber, int pageSize)
        => new(items, totalCount, pageNumber, pageSize);
}