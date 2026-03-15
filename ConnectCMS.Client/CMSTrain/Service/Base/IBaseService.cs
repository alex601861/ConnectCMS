using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;

namespace CMSTrain.Client.Service.Base;

public interface IBaseService : IScopedService
{
    Task<ResponseDto<T?>?> GetAsync<T>(string endpoint, 
        IList<string>? path = null,
        IDictionary<string, string?>? parameters = null, 
        IDictionary<string, string>? headersValue = null);

    Task<CollectionDto<T>?> GetPagedAsync<T>(string endpoint,
        IList<string>? path = null,
        IDictionary<string, string?>? parameters = null,
        IDictionary<string, string>? headersValue = null);

    Task<ResponseDto<T?>?> PostAsync<T>(string endpoint, StringContent stringContent, IList<string>? path = null);

    Task<ResponseDto<T?>?> UploadAsync<T>(string endpoint, string uploadType, MultipartFormDataContent formDataContent, IList<string>? path = null);

    Task<ResponseDto<T?>?> UpdateAsync<T>(string endpoint, string updateType, StringContent stringContent, IList<string>? path = null);

    Task<ResponseDto<T?>?> DeleteAsync<T>(string endpoint, string deleteType, IList<string>? path = null);
    
    Task<(byte[]? content, HttpResponseMessage? response)> DownloadAsync(string endpoint, 
        IList<string>? path = null, 
        IDictionary<string, string?>? parameters = null, 
        IDictionary<string, string?>? headersValue = null);
    
    Task<(byte[]? content, HttpResponseMessage? response)> DownloadAsync(string endpoint,
        MultipartFormDataContent formDataContent, 
        IList<string>? path = null,
        IDictionary<string, string?>? parameters = null, 
        IDictionary<string, string?>? headersValue = null);
    
    Task<(byte[]? content, HttpResponseMessage? response)> DownloadAsync(string endpoint,
        StringContent stringContent, 
        IList<string>? path = null,
        IDictionary<string, string?>? parameters = null, 
        IDictionary<string, string?>? headersValue = null);
}