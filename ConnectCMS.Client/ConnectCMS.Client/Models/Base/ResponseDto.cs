namespace CMSTrain.Client.Models.Base;

public class ResponseDto<T>: ResponseBaseDto<T>
{
    public ResponseDto() { }
}

public class ResponseBaseDto<T>
{
    public int StatusCode { get; set; }

    public string Message { get; set; }

    public T? Result { get; set; }
}