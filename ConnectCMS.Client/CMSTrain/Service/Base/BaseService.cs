using System.Net;
using MudBlazor;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.HTTP;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;

namespace CMSTrain.Client.Service.Base;

public class BaseService(ApiHttpClient apiHttpClient, 
    ILocalStorageService localStorageService, 
    NavigationManager navigationManager,
    ISnackbarService snackbarService) : IBaseService
{
    private readonly HttpClient _httpClient = apiHttpClient.HttpClient;
    
    public async Task<ResponseDto<T?>?> GetAsync<T>(string endpoint, 
        IList<string>? path = null,
        IDictionary<string, string?>? parameters = null, 
        IDictionary<string, string>? headersValue = null)
    {
        try
        {
            if (path is { Count: > 0 })
            {
                endpoint = path.Aggregate(endpoint, (current, parameter) => current + ("/" + parameter));
            }
            
            if (parameters is { Count: > 0 })
            {
                var queryString = string.Join("&", 
                    parameters.Where(kvp => kvp.Value != null)
                        .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value ?? string.Empty)}"));

                endpoint += "?" + queryString;
            }

            _httpClient.DefaultRequestHeaders.Clear();

            SetNgrokAccessibility();

            if (headersValue is { Count: > 0 })
            {
                foreach (var header in headersValue)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            await SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"/api/{endpoint}");

            var status = HandleResponse(response);
            
            if (status)
            {
                var result = await response.Content.ReadFromJsonAsync<ResponseDto<T?>>();

                if (result is not null)
                {
                    return HandleResponse(result);
                }
            }
            
            NavigateToLogin();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occured while handing your request: {ex.Message}");
        }

        return null;
    }
    
    public async Task<CollectionDto<T>?> GetPagedAsync<T>(string endpoint, 
        IList<string>? path = null,
        IDictionary<string, string?>? parameters = null, 
        IDictionary<string, string>? headersValue = null)
    {
        try
        {
            if (path is { Count: > 0 })
            {
                endpoint = path.Aggregate(endpoint, (current, parameter) => current + ("/" + parameter));
            }
            
            if (parameters is { Count: > 0 })
            {
                var queryString = string.Join("&", 
                    parameters.Where(kvp => kvp.Value != null)
                        .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value ?? string.Empty)}"));

                endpoint += "?" + queryString;
            }

            _httpClient.DefaultRequestHeaders.Clear();

            SetNgrokAccessibility();

            if (headersValue is { Count: > 0 })
            {
                foreach (var header in headersValue)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            await SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"/api/{endpoint}");

            var status = HandleResponse(response);
            
            if (status)
            {
                var result = await response.Content.ReadFromJsonAsync<CollectionDto<T>>();

                if (result is not null)
                {
                    return HandleResponse(result);
                }
            }
            
            NavigateToLogin();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occured while handing your request: {ex.Message}");
        }

        return null;
    }
    
    public async Task<ResponseDto<T?>?> PostAsync<T>(string endpoint, StringContent stringContent, IList<string>? path = null)
    {
        try
        {
            if (path is { Count: > 0 })
            {
                endpoint = path.Aggregate(endpoint, (current, parameter) => current + ("/" + parameter));
            }
            
            await SetAuthorizationHeader();

            SetNgrokAccessibility();

            var response = await _httpClient.PostAsync($"/api/{endpoint}", stringContent);
            
            var status = HandleResponse(response);
            
            if (status)
            {
                var result = await response.Content.ReadFromJsonAsync<ResponseDto<T?>>();

                if (result is not null)
                {
                    return HandleResponse(result);
                }
            }
            
            NavigateToLogin();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occured while handing your request: {ex.Message}");
        }

        return null;
    }

    public async Task<ResponseDto<T?>?> UploadAsync<T>(string endpoint, string uploadType, MultipartFormDataContent formDataContent, IList<string>? path = null)
    {
        try
        {
            if (path is { Count: > 0 })
            {
                endpoint = path.Aggregate(endpoint, (current, parameter) => current + ("/" + parameter));
            }
            
            await SetAuthorizationHeader();

            SetNgrokAccessibility();

            var response = uploadType == Constants.UploadType.Post
                ? await _httpClient.PostAsync($"/api/{endpoint}", formDataContent)
                : uploadType == Constants.UploadType.Put 
                    ? await _httpClient.PutAsync($"/api/{endpoint}", formDataContent)
                    : await _httpClient.PatchAsync($"/api/{endpoint}", formDataContent);

            var status = HandleResponse(response);
            
            if (status)
            {
                var result = await response.Content.ReadFromJsonAsync<ResponseDto<T?>>();

                if (result is not null)
                {
                    return HandleResponse(result);
                }
            }
            
            NavigateToLogin();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occured while handing your request: {ex.Message}");
        }

        return null;
    }

    public async Task<ResponseDto<T?>?> UpdateAsync<T>(string endpoint, string updateType, StringContent stringContent, IList<string>? path = null)
    {
        try
        {
            if (path is { Count: > 0 })
            {
                endpoint = path.Aggregate(endpoint, (current, parameter) => current + ("/" + parameter));
            }
            
            await SetAuthorizationHeader();

            SetNgrokAccessibility();

            var response = updateType == Constants.UpdateType.Patch 
                ? await _httpClient.PatchAsync($"/api/{endpoint}", stringContent) 
                : await _httpClient.PutAsync($"/api/{endpoint}", stringContent);

            var status = HandleResponse(response);
            
            if (status)
            {
                var result = await response.Content.ReadFromJsonAsync<ResponseDto<T?>>();

                if (result is not null)
                {
                    return HandleResponse(result);
                }
            }
            
            NavigateToLogin();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occured while handing your request: {ex.Message}");
        }

        return null;
    }

    public async Task<ResponseDto<T?>?> DeleteAsync<T>(string endpoint, string deleteType, IList<string>? path = null)
    {
        try
        {
            await SetAuthorizationHeader();

            if (path is { Count: > 0 })
            {
                endpoint = path.Aggregate(endpoint, (current, parameter) => current + ("/" + parameter));
            }

            SetNgrokAccessibility();

            var response = deleteType == Constants.DeleteType.Delete 
                ? await _httpClient.DeleteAsync($"api/{endpoint}")
                : await _httpClient.PatchAsync($"api/{endpoint}", null);

            var status = HandleResponse(response);
            
            if (status)
            {
                var result = await response.Content.ReadFromJsonAsync<ResponseDto<T?>>();

                if (result is not null)
                {
                    return HandleResponse(result);
                }
            }
            
            NavigateToLogin();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occured while handing your request: {ex.Message}");
        }

        return null;
    }
    
    public async Task<(byte[]? content, HttpResponseMessage? response)> DownloadAsync(string endpoint, IList<string>? path = null, IDictionary<string, string?>? parameters = null, IDictionary<string, string?>? headersValue = null)
    {
        try
        {
            if (path is { Count: > 0 })
            {
                endpoint = path.Aggregate(endpoint, (current, parameter) => current + ("/" + parameter));
            }
            
            if (parameters is { Count: > 0 })
            {
                var queryString = string.Join("&", 
                    parameters.Where(kvp => kvp.Value != null)
                        .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value ?? string.Empty)}"));
                
                endpoint += "?" + queryString;
            }

            _httpClient.DefaultRequestHeaders.Clear();

            SetNgrokAccessibility();

            if (headersValue is { Count: > 0 })
            {
                foreach (var header in headersValue)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            await SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"/api/{endpoint}");

            if (!response.IsSuccessStatusCode) return (null, response);
            
            var content = await response.Content.ReadAsByteArrayAsync();
                
            return (content, response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occured while handing your request: {ex.Message}");
        }

        return (null, null);
    }

    public async Task<(byte[]? content, HttpResponseMessage? response)> DownloadAsync(string endpoint, 
        MultipartFormDataContent formDataContent,
        IList<string>? path = null, 
        IDictionary<string, string?>? parameters = null, 
        IDictionary<string, string?>? headersValue = null)
    {
        try
        {
            if (path is { Count: > 0 })
            {
                endpoint = path.Aggregate(endpoint, (current, parameter) => current + ("/" + parameter));
            }
            
            if (parameters is { Count: > 0 })
            {
                var queryString = string.Join("&", 
                    parameters.Where(kvp => kvp.Value != null)
                        .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value ?? string.Empty)}"));
                
                endpoint += "?" + queryString;
            }

            _httpClient.DefaultRequestHeaders.Clear();

            SetNgrokAccessibility();

            if (headersValue is { Count: > 0 })
            {
                foreach (var header in headersValue)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            await SetAuthorizationHeader();

            var response = await _httpClient.PostAsync($"/api/{endpoint}", formDataContent);

            var status = HandleResponse(response);
            
            if (status)
            {
                if (!response.IsSuccessStatusCode) return (null, response);
            
                var content = await response.Content.ReadAsByteArrayAsync();
                
                return (content, response);
            }
            
            NavigateToLogin();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occured while handing your request: {ex.Message}");
        }

        return (null, null);
    }

    public async Task<(byte[]? content, HttpResponseMessage? response)> DownloadAsync(string endpoint,
        StringContent stringContent,
        IList<string>? path = null,
        IDictionary<string, string?>? parameters = null,
        IDictionary<string, string?>? headersValue = null)
    {
        try
        {
            if (path is { Count: > 0 })
            {
                endpoint = path.Aggregate(endpoint, (current, parameter) => current + ("/" + parameter));
            }
            
            if (parameters is { Count: > 0 })
            {
                var queryString = string.Join("&", 
                    parameters.Where(kvp => kvp.Value != null)
                        .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value ?? string.Empty)}"));
                
                endpoint += "?" + queryString;
            }

            _httpClient.DefaultRequestHeaders.Clear();

            SetNgrokAccessibility();

            if (headersValue is { Count: > 0 })
            {
                foreach (var header in headersValue)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            await SetAuthorizationHeader();

            var response = await _httpClient.PostAsync($"/api/{endpoint}", stringContent);

            var status = HandleResponse(response);
            
            if (status)
            {
                if (!response.IsSuccessStatusCode) return (null, response);
            
                var content = await response.Content.ReadAsByteArrayAsync();
                
                return (content, response);
            }
            
            NavigateToLogin();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occured while handing your request: {ex.Message}");
        }

        return (null, null);
    }
    
    private async Task SetAuthorizationHeader()
    {
        var token = await localStorageService.GetItemAsync<string>(Constants.LocalStorage.Token);

        if (token != null)
        {
            var accessToken = StringCipher.Decrypt(token, Constants.Encryption.Key);
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    private void SetNgrokAccessibility()
    {
        _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
    }
    
    private ResponseDto<T?> HandleResponse<T>(ResponseDto<T?> response)
    {
        if (response.StatusCode != StatusCode.Status401Unauthorized) return response;

        response.Message = Constants.Message.UnauthorizedMessage;
        
        NavigateToLogin();
        
        return response;
    }
    
    private CollectionDto<T> HandleResponse<T>(CollectionDto<T> response)
    {
        if (response.StatusCode != StatusCode.Status401Unauthorized) return response;

        response.Message = Constants.Message.UnauthorizedMessage;
        
        NavigateToLogin();
        
        return response;
    }
    
    private static bool HandleResponse(HttpResponseMessage response)
    {
        return response.StatusCode is not (HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden);
    }

    private void NavigateToLogin()
    {
        snackbarService.ShowSnackbar(Constants.Message.UnauthorizedMessage, Severity.Warning, Variant.Outlined);
            
        navigationManager.NavigateTo("login");
    }
}