using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components.Authorization;

namespace CMSTrain.Client.Service.Manager;

public class IdentityAuthenticationStateManager(ILocalStorageManager localStorageManager, JwtSecurityTokenHandler jwtSecurityTokenHandler) : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await localStorageManager.GetItemAsync<string>(Constants.LocalStorage.Token);
        
        if (string.IsNullOrWhiteSpace(token)) return new AuthenticationState(_anonymous);

        try
        {
            var accessToken = StringCipher.Decrypt(token, Constants.Encryption.Key);
            
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(accessToken);

            if (tokenContent.ValidTo < DateTime.UtcNow)
            {
                await localStorageManager.ClearItemAsync(Constants.LocalStorage.Token);
                
                return new AuthenticationState(_anonymous);
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(tokenContent.Claims, Constants.LocalStorage.Jwt));
            
            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task LoggedIn()
    {
        var authState = await GetAuthenticationStateAsync();
        
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public async Task LoggedOut()
    {
        await localStorageManager.ClearItemAsync(Constants.LocalStorage.Token);

        var authState = Task.FromResult(new AuthenticationState(_anonymous));
        
        NotifyAuthenticationStateChanged(authState);
    }

    private async Task<List<Claim>> GetClaims()
    {
        var token = await localStorageManager.GetItemAsync<string>(Constants.LocalStorage.Token);

        if (string.IsNullOrEmpty(token)) return [];
        
        var accessToken = StringCipher.Decrypt(token, Constants.Encryption.Key);
        
        var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(accessToken);
        
        return tokenContent.Claims.ToList();
    }
}