using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Email;

namespace CMSTrain.Client.Service.Interface;

public interface IEmailConfirmationService : ITransientService
{
    #region Authentication & Profile
    Task<ResponseDto<bool?>?> SelfRegistration(RegistrationEmailRequestDto registrationDetails);

    Task<ResponseDto<bool?>?> UserRegistration(UserRegistrationRequestDto registrationDetails);

    Task<ResponseDto<bool?>?> ClientCandidateRegistration(UserRegistrationRequestDto registrationDetails);
    
    Task<ResponseDto<bool?>?> ForgotPassword(ForgotPasswordRequestDto forgotPasswordRequest);

    Task<ResponseDto<bool?>?> ResetPassword(ResetPasswordRequestDto resetPasswordRequest);
    #endregion
    
    #region Training Requests and Actions
    Task<ResponseDto<bool?>?> TrainingRequest(TrainingRequestsRequestDto trainingRequest);

    Task<ResponseDto<bool?>?> TrainingRequestAction(TrainingRequestsActionRequestDto trainingRequest);
    #endregion
}