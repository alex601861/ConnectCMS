using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.EmailConfirmation;
using Microsoft.AspNetCore.Authorization;

namespace CMSTrain.Controllers;

[Route("api/email-confirmation")]
public class EmailConfirmationController(IEmailConfirmationService emailConfirmationService) : BaseController<EmailConfirmationController>
{
    #region Authentication and Profile
    [AllowAnonymous]
    [HttpPost("self-registration")]
    public async Task<IActionResult> SelfRegistration(RegistrationEmailRequestDto registrationEmail)
    {
        await emailConfirmationService.SelfRegistration(registrationEmail);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "A confirmation email has been successfully sent, please check your inbox and follow the given instructions.",
            Result = true,
        });
    }

    [HttpPost("user-registration")]
    public async Task<IActionResult> UserRegistration(UserRegistrationRequestDto registrationEmail)
    {
        await emailConfirmationService.UserRegistration(registrationEmail);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "A confirmation email has been successfully sent to the respective user with his/her respective credentials and profile details.",
            Result = true,
        });
    }

    [HttpPost("client-candidate-registration")]
    public async Task<IActionResult> ClientCandidateRegistration(UserRegistrationRequestDto registrationEmail)
    {
        await emailConfirmationService.ClientCandidateRegistration(registrationEmail);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "A confirmation email has been successfully sent to the respective user with his/her respective credentials and profile details.",
            Result = true,
        });
    }
    
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto forgotPassword)
    {
        await emailConfirmationService.ForgotPassword(forgotPassword);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "A confirmation email to reset your password has been successfully sent, please check your inbox and follow the given instructions.",
            Result = true,
        });
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetUserPassword(ResetPasswordRequestDto resetUserPassword)
    {
        await emailConfirmationService.ResetPassword(resetUserPassword);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "A confirmation email with the newly updated password has been successfully sent to the respective user.",
            Result = true,
        });
    }
    #endregion

    #region Training Requests and Actions
    [HttpPost("training-request")]
    public async Task<IActionResult> TrainingRequest(TrainingRequestsRequestDto trainingRequestsRequest)
    {
        await emailConfirmationService.TrainingRequest(trainingRequestsRequest);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Confirmation email successfully sent, please check your inbox and follow the given instructions.",
            Result = true,
        });
    }
    
    [HttpPost("training-request-action")]
    public async Task<IActionResult> TrainingRequestAction(TrainingRequestsActionRequestDto trainingRequestsActionRequest)
    {
        await emailConfirmationService.TrainingRequestAction(trainingRequestsActionRequest);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Confirmation email has been successfully sent.",
            Result = true,
        });
    }
    #endregion
}