using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Preferences;

namespace CMSTrain.Client.Service.Manager;

public interface IPreferenceManager : ITransientService
{
    Task<IPreference> GetPreference();
    
    Task SetPreference(IPreference preference);
}