namespace CMSTrain.Client.Service.HTTP;

public class LocalHttpClient
{
    public HttpClient HttpClient { get; }

    public LocalHttpClient(HttpClient httpClient, string baseUrl)
    {
        HttpClient = httpClient;
        HttpClient.BaseAddress = new Uri(baseUrl);
    }
}