using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Models.Responses.Identity;

namespace CMSTrain.Client.Service.Interface;

public interface IAuthenticationService : ITransientService
{
    Task<bool> IsUserLoggedIn();
    
    Task SetUpAccessToken(string accessToken);

    Task SetUpReturnUrl(string returnUrl);

    Task<string?> GetReturnUrl();
    
    Task<ResponseDto<UserLoginResponseDto?>?> Login(LoginDto loginDto);

    Task<ResponseDto<RegistrationResponseDto?>?> UserRegister(UserRegisterDto userRegistration);
    
    Task<ResponseDto<RegistrationResponseDto?>?> SelfRegister(CandidateRegisterDto userRegistration);

    Task<ResponseDto<RegistrationResponseDto?>?> ClientCandidateRegister(CandidateRegisterDto userRegistration);
    
    Task<ResponseDto<bool?>?> VerifyEmailConfirmation(EmailVerificationRequestDto emailVerification);

    Task<ResponseDto<bool?>?> ResetPassword(ForgotPasswordEmailRequestDto forgotPasswordEmail);
    
    Task<ResponseDto<ResetUserPasswordRequestDto?>?> ResetUserPassword(ResetUserPasswordDto resetUserPassword);
    
    Task LogOut();

    Task ClearNavigationUrl();
}