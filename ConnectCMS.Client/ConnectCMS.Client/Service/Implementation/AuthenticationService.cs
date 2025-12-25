using System.Text.Json;
using System.Net.Http.Headers;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using System.IdentityModel.Tokens.Jwt;
using CMSTrain.Client.Service.Manager;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Models.Responses.Identity;
using Microsoft.AspNetCore.Components.Authorization;

namespace CMSTrain.Client.Service.Implementation;

public class AuthenticationService(IBaseService baseService, ILocalStorageManager localStorageManager, AuthenticationStateProvider authenticationStateProvider) : IAuthenticationService
{
    public async Task<bool> IsUserLoggedIn()
    {
        var token = await localStorageManager.GetItemAsync<string>(Constants.LocalStorage.Token);
        
        if (token == null) return false;

        var accessToken = StringCipher.Decrypt(token, Constants.Encryption.Key);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var jwtToken = tokenHandler.ReadJwtToken(accessToken);
        
        var expiryDateTime = jwtToken.ValidTo;
         
        return expiryDateTime > DateTime.UtcNow;
    }

    public async Task SetUpAccessToken(string accessToken)
    {
        await localStorageManager.ClearItemAsync(Constants.LocalStorage.Token);

        await localStorageManager.SetItemAsync(Constants.LocalStorage.Token, accessToken);
    }

    public async Task SetUpReturnUrl(string returnUrl)
    {
        await localStorageManager.SetItemAsync(Constants.LocalStorage.Navigation, returnUrl);
    }

    public async Task<string?> GetReturnUrl()
    {
        var returnUrl = await localStorageManager.GetItemAsync<string>(Constants.LocalStorage.Navigation);
        
        if (returnUrl != null) await localStorageManager.ClearItemAsync(Constants.LocalStorage.Navigation);
        
        return returnUrl;
    }
    
    public async Task<ResponseDto<UserLoginResponseDto?>?> Login(LoginDto loginDto)
    {
        var jsonRequest = JsonSerializer.Serialize(loginDto);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.PostAsync<UserLoginResponseDto?>(ApiEndpoints.Authentication.Login, content);

        if (string.IsNullOrEmpty(response?.Result?.Token)) return response;

        await localStorageManager.SetItemAsync(Constants.LocalStorage.Token, response.Result.Token);

        await ((IdentityAuthenticationStateManager)authenticationStateProvider).LoggedIn();

        return response;
    }
    
    public async Task<ResponseDto<RegistrationResponseDto?>?> UserRegister(UserRegisterDto userRegistration)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(userRegistration.Name ?? ""), "Name");
        formData.Add(new StringContent(userRegistration.Email ?? ""), "Email");
        formData.Add(new StringContent(userRegistration.Password ?? ""), "Password");
        formData.Add(new StringContent(userRegistration.PhoneNumber ?? ""), "PhoneNumber");
        formData.Add(new StringContent(userRegistration.RoleId.ToString()), "RoleId");
        formData.Add(new StringContent(userRegistration.OrganizationId.ToString()), "OrganizationId");
        formData.Add(new StringContent(userRegistration.ConfirmPassword ?? ""), "ConfirmPassword");
        formData.Add(new StringContent(userRegistration.Gender.ToString() ?? ""), "Gender");
        formData.Add(new StringContent(userRegistration.Address ?? ""), "Address");
        
        if (userRegistration.DesignationId != Guid.Empty || userRegistration.DesignationId != null)
        {
            formData.Add(new StringContent(userRegistration.DesignationId.ToString() ?? string.Empty), "DesignationId");
        }

        if (userRegistration.CountryId != Guid.Empty || userRegistration.CountryId != null)
        {
            formData.Add(new StringContent(userRegistration.CountryId.ToString() ?? string.Empty), "CountryId");
        }
        
        if (userRegistration.ImageUrl != null)
        {
            var organizationFileContent = new StreamContent(userRegistration.ImageUrl!.OpenReadStream(long.MaxValue));
            
            organizationFileContent.Headers.ContentType = new MediaTypeHeaderValue(userRegistration.ImageUrl.ContentType);
            
            formData.Add(organizationFileContent, "ImageUrl", userRegistration.ImageUrl.Name);
        }

        var response = await baseService.UploadAsync<RegistrationResponseDto?>(ApiEndpoints.Authentication.UserRegister, Constants.UploadType.Post, formData);

        return response;
    }

    public async Task<ResponseDto<RegistrationResponseDto?>?> SelfRegister(CandidateRegisterDto userRegistration)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(userRegistration.Name ?? ""), "Name");
        formData.Add(new StringContent(userRegistration.Email ?? ""), "Email");
        formData.Add(new StringContent(userRegistration.Password ?? ""), "Password");
        formData.Add(new StringContent(userRegistration.PhoneNumber ?? ""), "PhoneNumber");
        formData.Add(new StringContent(userRegistration.ConfirmPassword ?? ""), "ConfirmPassword");
        formData.Add(new StringContent(userRegistration.Gender.ToString() ?? ""), "Gender");

        formData.Add(new StringContent(userRegistration.Address ?? ""), "Address");
        
        if (userRegistration.DesignationId != Guid.Empty || userRegistration.DesignationId != null)
        {
            formData.Add(new StringContent(userRegistration.DesignationId.ToString() ?? string.Empty), "DesignationId");
        }
        
        if (userRegistration.ImageUrl != null)
        {
            var organizationFileContent = new StreamContent(userRegistration.ImageUrl!.OpenReadStream(long.MaxValue));
            
            organizationFileContent.Headers.ContentType = new MediaTypeHeaderValue(userRegistration.ImageUrl.ContentType);
            
            formData.Add(organizationFileContent, "ImageUrl", userRegistration.ImageUrl.Name);
        }
        
        var response = await baseService.UploadAsync<RegistrationResponseDto?>(ApiEndpoints.Authentication.SelfRegister, Constants.UploadType.Post, formData);

        return response;
    }

    public async Task<ResponseDto<RegistrationResponseDto?>?> ClientCandidateRegister(CandidateRegisterDto userRegistration)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(userRegistration.Name ?? ""), "Name");
        formData.Add(new StringContent(userRegistration.Email ?? ""), "Email");
        formData.Add(new StringContent(userRegistration.Password ?? ""), "Password");
        formData.Add(new StringContent(userRegistration.PhoneNumber ?? ""), "PhoneNumber");
        formData.Add(new StringContent(userRegistration.ConfirmPassword ?? ""), "ConfirmPassword");
        formData.Add(new StringContent(userRegistration.Gender.ToString() ?? ""), "Gender");
        formData.Add(new StringContent(userRegistration.Address ?? ""), "Address");
        
        if (userRegistration.DesignationId != Guid.Empty || userRegistration.DesignationId != null)
        {
            formData.Add(new StringContent(userRegistration.DesignationId.ToString() ?? string.Empty), "DesignationId");
        }
        
        if (userRegistration.ImageUrl != null)
        {
            var organizationFileContent = new StreamContent(userRegistration.ImageUrl!.OpenReadStream(long.MaxValue));
            
            organizationFileContent.Headers.ContentType = new MediaTypeHeaderValue(userRegistration.ImageUrl.ContentType);
            
            formData.Add(organizationFileContent, "ImageUrl", userRegistration.ImageUrl.Name);
        }

        var response = await baseService.UploadAsync<RegistrationResponseDto?>(ApiEndpoints.Authentication.ClientCandidateRegister, Constants.UploadType.Post, formData);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> VerifyEmailConfirmation(EmailVerificationRequestDto emailVerificationDto)
    {
        var jsonRequest = JsonSerializer.Serialize(emailVerificationDto);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Authentication.VerifyEmailConfirmation, content);
        
        return response;
    }
    
    public async Task<ResponseDto<bool?>?> ResetPassword(ForgotPasswordEmailRequestDto forgotPasswordEmail)
    {
        var jsonRequest = JsonSerializer.Serialize(forgotPasswordEmail);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Authentication.ResetPassword, content);
        
        return response;
    }
    
    public async Task<ResponseDto<ResetUserPasswordRequestDto?>?> ResetUserPassword(ResetUserPasswordDto resetUserPassword)
    {
        var jsonRequest = JsonSerializer.Serialize(resetUserPassword);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<ResetUserPasswordRequestDto?>(ApiEndpoints.Authentication.ResetUserPassword, content);
        
        return response;
    }
    
    public async Task LogOut()
    {
        var token = await localStorageManager.GetItemAsync<string>(Constants.LocalStorage.Token);
        
        if (!string.IsNullOrEmpty(token))
        {
            var content = new StringContent("", System.Text.Encoding.UTF8, "application/json");
            
            await baseService.PostAsync<bool>(ApiEndpoints.Authentication.Logout, content);
            
            await localStorageManager.ClearItemAsync(Constants.LocalStorage.Token);
            
            await ((IdentityAuthenticationStateManager)authenticationStateProvider).LoggedOut(); 
        }
    }

    public async Task ClearNavigationUrl()
    {
        var returnUrl = await localStorageManager.GetItemAsync<string>(Constants.LocalStorage.Navigation);

        if (!string.IsNullOrEmpty(returnUrl))
        {
            await localStorageManager.ClearItemAsync(Constants.LocalStorage.Navigation);
        }
    }
}