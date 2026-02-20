using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.EmailConfirmation;
using CMSTrain.Application.DTOs.Identity;

namespace CMSTrain.Application.Interfaces.Services.Identity;

public interface IAuthenticationService : ITransientService
{
    Task<UserLoginResponseDto> Login(LoginDto login);

    Task<RegistrationResponseDto> SelfCandidateRegister(CandidateRegisterDto candidate);

    Task<RegistrationResponseDto> ClientCandidateRegister(CandidateRegisterDto candidate);

    Task<RegistrationResponseDto> UserRegister(UserRegisterDto user);

    Task VerifyEmailConfirmation(EmailVerificationRequestDto emailVerification);

    Task ResetPassword(ForgotPasswordEmailRequestDto verifyPasswordDto);
    
    Task<ResetPasswordRequestDto> ResetUserPassword(ResetUserPasswordDto resetUserPassword);

    void ExpireToken(string token);

    bool IsTokenExpired(string token);
}