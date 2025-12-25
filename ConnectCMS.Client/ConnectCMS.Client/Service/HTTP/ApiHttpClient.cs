namespace CMSTrain.Client.Service.HTTP;

public class ApiHttpClient
{
    public HttpClient HttpClient { get; }

    public ApiHttpClient(HttpClient httpClient, string apiUrl)
    {
        HttpClient = httpClient;
        HttpClient.BaseAddress = new Uri(apiUrl);
    }
}