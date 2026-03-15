using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Email;

namespace CMSTrain.Client.Service.Implementation;

public class EmailConfirmationService(IBaseService baseService) : IEmailConfirmationService
{
    public async Task<ResponseDto<bool?>?> SelfRegistration(RegistrationEmailRequestDto registrationDetails)
    {
        var jsonRequest = JsonSerializer.Serialize(registrationDetails);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.EmailConfirmation.SelfRegistration, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> UserRegistration(UserRegistrationRequestDto registrationDetails)
    {
        var jsonRequest = JsonSerializer.Serialize(registrationDetails);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.EmailConfirmation.UserRegistration, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> ClientCandidateRegistration(UserRegistrationRequestDto registrationDetails)
    {
        var jsonRequest = JsonSerializer.Serialize(registrationDetails);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.EmailConfirmation.ClientCandidateRegistration, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> ForgotPassword(ForgotPasswordRequestDto forgotPasswordRequest)
    {
        var jsonRequest = JsonSerializer.Serialize(forgotPasswordRequest);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.EmailConfirmation.ForgotPassword, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> ResetPassword(ResetPasswordRequestDto resetPasswordRequest)
    {
        var jsonRequest = JsonSerializer.Serialize(resetPasswordRequest);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.EmailConfirmation.ResetUserPassword, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> TrainingRequest(TrainingRequestsRequestDto trainingRequest)
    {
        var jsonRequest = JsonSerializer.Serialize(trainingRequest);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.EmailConfirmation.TrainingRequest, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> TrainingRequestAction(TrainingRequestsActionRequestDto trainingRequest)
    {
        var jsonRequest = JsonSerializer.Serialize(trainingRequest);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.EmailConfirmation.TrainingRequestAction, content);
        
        return response;
    }
}