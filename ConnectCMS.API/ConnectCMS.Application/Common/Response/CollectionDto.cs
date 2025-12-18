namespace CMSTrain.Application.Common.Response;

public class CollectionDto<T>(List<T> items, int count, int pageNumber, int pageSize) where T : class
{
    public int StatusCode { get; set; }

    public string Message { get; set; }

    public int CurrentPage { get; set; } = pageNumber;

    public int TotalPages { get; set; } = (int)Math.Ceiling(count / (double)pageSize);

    public int PageSize { get; set; } = pageSize;

    public int TotalCount { get; set; } = count;

    public int DisplayCount { get; set; } = items.Count;

    public IEnumerable<T> Result { get; set; } = items;
}