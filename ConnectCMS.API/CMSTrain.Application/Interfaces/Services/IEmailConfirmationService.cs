using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.EmailConfirmation;

namespace CMSTrain.Application.Interfaces.Services;

public interface IEmailConfirmationService : ITransientService
{
    #region Authentication and Profile
    Task SelfRegistration(RegistrationEmailRequestDto registrationEmail);

    Task UserRegistration(UserRegistrationRequestDto registrationEmail);

    Task ClientCandidateRegistration(UserRegistrationRequestDto registrationEmail);
    
    Task ForgotPassword(ForgotPasswordRequestDto forgotPasswordDto);

    Task ResetPassword(ResetPasswordRequestDto forgotPasswordDto);
    #endregion

    #region Training Requests and Actions
    Task TrainingRequest(TrainingRequestsRequestDto trainingRequestsRequest);

    Task TrainingRequestAction(TrainingRequestsActionRequestDto trainingRequestsActionRequest);
    #endregion
}
