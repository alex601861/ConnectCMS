namespace CMSTrain.Client.Models.Base;

public class CollectionDto<T>: CollectionBaseDto<T>
{
    public CollectionDto() { }
}

public class CollectionBaseDto<T>
{
    public int StatusCode { get; set; }

    public string Message { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int DisplayCount { get; set; }
    
    public List<T> Result { get; set; }
}