using Blazored.LocalStorage;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Preferences;

namespace CMSTrain.Client.Service.Manager;

public class ClientPreferenceManager(ILocalStorageService localStorageService) : IClientPreferenceManager
{
    public async Task<IPreference> GetPreference()
    {
        return await localStorageService.GetItemAsync<ClientPreference>(Constants.LocalStorage.Preference) ?? new ClientPreference();
    }

    public async Task SetPreference(IPreference preference)
    {
        await localStorageService.SetItemAsync(Constants.LocalStorage.Preference, preference as ClientPreference);
    }
}