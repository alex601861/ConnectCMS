using Blazored.LocalStorage;

namespace CMSTrain.Client.Service.Manager;

public class LocalStorageManager(ILocalStorageService localStorage) : ILocalStorageManager
{
    public async Task<T?> GetItemAsync<T>(string key)
    {
        return await localStorage.GetItemAsync<T>(key);
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        var item = await GetItemAsync<T>(key);
        
        if (item != null) await ClearItemAsync(key);
        
        await localStorage.SetItemAsync(key, value);
    }

    public async Task ClearItemAsync(string key)
    {
        await localStorage.RemoveItemAsync(key);
    }
}