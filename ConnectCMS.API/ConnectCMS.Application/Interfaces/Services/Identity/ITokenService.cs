using CMSTrain.Application.Common.Service;

namespace CMSTrain.Application.Interfaces.Services.Identity;

public interface ITokenService : ITransientService
{
    Task<bool> IsCurrentActiveToken();
    
    Task DeactivateCurrentAsync();
    
    Task<bool> IsActiveAsync(string token);
    
    Task DeactivateAsync(string token);
}