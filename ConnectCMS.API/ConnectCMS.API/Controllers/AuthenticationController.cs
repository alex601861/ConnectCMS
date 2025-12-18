using Microsoft.Net.Http.Headers;
using CMSTrain.Application.DTOs.Identity;
using Microsoft.AspNetCore.Authorization;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.DTOs.EmailConfirmation;
using CMSTrain.Application.Interfaces.Services.Identity;

namespace CMSTrain.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController(IAuthenticationService authenticationService) : BaseController<AuthenticationController>
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginRequest)
    {
        var result = await authenticationService.Login(loginRequest);

        return Ok(new ResponseDto<UserLoginResponseDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully authenticated.",
            Result = result,
        });
    }

    [HttpPost("user-registration")]
    public async Task<IActionResult> UserRegister([FromForm] UserRegisterDto registration)
    {
        var result = await authenticationService.UserRegister(registration);

        return Ok(new ResponseDto<RegistrationResponseDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "User successfully registered, please wait as we are sending a confirmation email.",
            Result = result,
        });
    }

    [AllowAnonymous]
    [HttpPost("self-registration")]
    public async Task<IActionResult> SelfRegister([FromForm] CandidateRegisterDto registration)
    {
        var result = await authenticationService.SelfCandidateRegister(registration);

        return Ok(new ResponseDto<RegistrationResponseDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Candidate successfully registered, please wait as we are sending a confirmation email.",
            Result = result
        });
    }

    [HttpPost("client-candidate-registration")]
    public async Task<IActionResult> ClientCandidateRegister([FromForm] CandidateRegisterDto registration)
    {
        var result = await authenticationService.ClientCandidateRegister(registration);

        return Ok(new ResponseDto<RegistrationResponseDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Candidate successfully registered, please wait as we are sending a confirmation email.",
            Result = result,
        });
    }

    [AllowAnonymous]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> VerifyEmailConfirmation(EmailVerificationRequestDto emailVerificationDto)
    {
        await authenticationService.VerifyEmailConfirmation(emailVerificationDto);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Email successfully verified.",
            Result = true,
        });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ForgotPasswordEmailRequestDto forgotPasswordEmail)
    {
        await authenticationService.ResetPassword(forgotPasswordEmail);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Password for the following user has been successfully reset.",
            Result = true,
        });
    }
    
    [HttpPost("reset-user-password")]
    public async Task<IActionResult> ResetUserPassword(ResetUserPasswordDto resetUserPassword)
    {
        var result = await authenticationService.ResetUserPassword(resetUserPassword);

        return Ok(new ResponseDto<ResetPasswordRequestDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Password for the following user has been successfully reset.",
            Result = result,
        });
    }

    [HttpPost("logout")]
    public Task<IActionResult> Logout()
    {
        var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

        authenticationService.ExpireToken(accessToken);

        var result = new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "User successfully logged out.",
            Result = true
        };

        return Task.FromResult<IActionResult>(Ok(result));
    }
}
