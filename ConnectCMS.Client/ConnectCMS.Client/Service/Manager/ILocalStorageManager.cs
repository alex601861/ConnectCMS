using CMSTrain.Client.Service.Dependency;

namespace CMSTrain.Client.Service.Manager;

public interface ILocalStorageManager : ITransientService
{
    Task<T?> GetItemAsync<T>(string key);

    Task SetItemAsync<T>(string key, T value);
    
    Task ClearItemAsync(string key);
}