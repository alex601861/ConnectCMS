using CMSTrain.Helper;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using CMSTrain.Application.Settings;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.DTOs.Email;
using CMSTrain.Application.Common.User;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.EmailConfirmation;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class EmailConfirmationService(
    IEmailService emailService,
    UserManager<User> userManager,
    IGenericRepository genericRepository,
    ICurrentUserService currentUserService,
    IOptions<ClientSettings> clientSettings) : IEmailConfirmationService
{
    private readonly string _baseUrl = clientSettings.Value.BaseUrl.Split(";").FirstOrDefault() 
        ?? throw new NotFoundException("The Base URL has not been stabilized and initialized");

    public async Task SelfRegistration(RegistrationEmailRequestDto registrationEmail)
    {
        if (registrationEmail.UserId == null && registrationEmail.EmailAddress == null)
        {
            throw new BadRequestException("A confirmation email could not be sent to the following user's address",
                ["Both the user identifier and email address cannot be null."]);
        }
        
        var user = registrationEmail.UserId is null
            ? genericRepository.GetFirstOrDefault<User>(x => x.Email == registrationEmail.EmailAddress) ??
                throw new NotFoundException("The following user has not been registered to our system.")
            : genericRepository.GetById<User>(Guid.Parse(registrationEmail.UserId)) ??
                throw new NotFoundException("The following user has not been registered to our system.");

        var verificationCode = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = System.Web.HttpUtility.UrlEncode(verificationCode);

        var baseUrl = $"{_baseUrl}/{Constants.Navigation.SelfRegistration}";

        var emailDto = new EmailDto
        {
            FullName = user.Name,
            ToEmailAddress = user.Email ?? "",
            Subject = "Connect CMS - Registration Email Verification",
            PrimaryMessage = $"{baseUrl}/{encodedToken}/{user.Id}",
            EmailProcess = EmailProcess.SelfRegistration,
            UserName = user.UserName ?? "",
        };

        await emailService.SendEmail(emailDto);
    }

    public async Task UserRegistration(UserRegistrationRequestDto registrationEmail)
    {
        if (registrationEmail.UserId == null && registrationEmail.EmailAddress == null)
        {
            throw new BadRequestException("A confirmation email could not be sent to the following user's address",
                ["Both the user identifier and email address cannot be null."]);
        }

        var user = registrationEmail.UserId is null
            ? genericRepository.GetFirstOrDefault<User>(x => x.Email == registrationEmail.EmailAddress) 
              ?? throw new NotFoundException("The following user has not been registered to our system.")
            : genericRepository.GetById<User>(Guid.Parse(registrationEmail.UserId)) 
              ?? throw new NotFoundException("The following user has not been registered to our system.");

        var userRole = genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == user.Id)
                       ?? throw new NotFoundException("The following user has not been assigned to any role.");
        
        var role = genericRepository.GetById<Role>(userRole.RoleId)
                   ?? throw new NotFoundException("The following user has not been assigned to any role.");

        var primaryMessage = string.Empty;

        switch (role.Name)
        {
            case Constants.Roles.Client when user.OrganizationId == null:
                throw new NotFoundException("The following user has not been assigned to any organization.");
            case Constants.Roles.Candidate when user.OrganizationId == null:
                primaryMessage = "You have been assigned as a self-registered candidate. Your roles and responsibilities will be discussed briefly in the upcoming training session.";
                break;
            case Constants.Roles.Client:
            {
                var organization = genericRepository.GetById<Organization>(user.OrganizationId)
                                   ?? throw new NotFoundException("The following user has not been assigned to any organization.");
            
                primaryMessage = $"You have been assigned as an Client Organization Administrator for {organization.Name}. Your roles and responsibilities will be discussed briefly in the upcoming training session.";
                break;
            }
            case Constants.Roles.Candidate:
            {
                var organization = genericRepository.GetById<Organization>(user.OrganizationId)
                                   ?? throw new NotFoundException("The following user has not been assigned to any organization.");
            
                primaryMessage = $"You have been nominated as an Client Registered Candidate, nominated for {organization.Name}. Your roles and responsibilities will be discussed briefly in the upcoming training session.";
                break;
            }
        }

        var emailDto = new EmailDto
        {
            FullName = user.Name,
            ToEmailAddress = user.Email ?? "",
            Subject = $"Connect CMS - {role.Name} Registration",
            EmailProcess = EmailProcess.UserRegistration,
            UserName = user.UserName ?? "",
            Password = registrationEmail.Password,
            PrimaryMessage = primaryMessage,
            Role = role.Name,
        };

        await emailService.SendEmail(emailDto);
    }
    
    public async Task ClientCandidateRegistration(UserRegistrationRequestDto registrationEmail)
    {
        if (registrationEmail.UserId == null && registrationEmail.EmailAddress == null)
        {
            throw new BadRequestException("A confirmation email could not be sent to the following user's address",
                ["Both the user identifier and email address cannot be null."]);
        }

        var user = registrationEmail.EmailAddress is not null
            ? genericRepository.GetFirstOrDefault<User>(x => x.Email != null && x.Email.ToLower() == registrationEmail.EmailAddress.ToLower()) ??
                throw new NotFoundException("The following user has not been registered to our system.")
            : registrationEmail.UserId is not null 
                ? genericRepository.GetById<User>(Guid.Parse(registrationEmail.UserId)) ??
                  throw new NotFoundException("The following user has not been registered to our system.")
                  : throw new BadRequestException("A confirmation email could not be sent to the following user's address",
                      ["Both the user identifier and email address cannot be null."]);

        var userRole = genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == user.Id)
                       ?? throw new NotFoundException("The following user has not been assigned to any role.");
        
        var role = genericRepository.GetById<Role>(userRole.RoleId)
                   ?? throw new NotFoundException("The following user has not been assigned to any role.");

        var primaryMessage = string.Empty;
        
        if (role.Name == Constants.Roles.Candidate)
        {
            if (user.OrganizationId == null)
                throw new NotFoundException("The following user has not been assigned to any organization.");
            
            var organization = genericRepository.GetById<Organization>(user.OrganizationId)
                               ?? throw new NotFoundException("The following user has not been assigned to any organization.");
        
            primaryMessage = $"You have been nominated as an Client Registered Candidate, nominated for {organization.Name}. Your roles and responsibilities will be discussed briefly in the upcoming training session.";
        }
                
        var emailDto = new EmailDto
        {
            FullName = user.Name,
            ToEmailAddress = user.Email ?? "",
            Subject = "Connect CMS - User Registration",
            EmailProcess = EmailProcess.ClientCandidateRegistration,
            UserName = user.UserName ?? "",
            Password = registrationEmail.Password,
            Role = "Client Nominated Candidate",
            PrimaryMessage = primaryMessage
        };

        await emailService.SendEmail(emailDto);
    }
    
    public async Task ForgotPassword(ForgotPasswordRequestDto forgotPassword)
    {
        var user = genericRepository.GetFirstOrDefault<User>(x => x.Email == forgotPassword.EmailAddress) ??
              throw new NotFoundException("The following user has not been registered to our system.");

        if (!user.EmailConfirmed)
        {
            throw new BadRequestException("The following user has not been confirmed to our system.",
                ["Please confirm your email address before proceeding with the password reset."]);
        }
        
        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var encodedToken = System.Web.HttpUtility.UrlEncode(token);

        var baseUrl = $"{_baseUrl}/{Constants.Navigation.ResetPassword}";

        var emailDto = new EmailDto
        {
            FullName = user.Name,
            ToEmailAddress = user.Email ?? "",
            Subject = "Connect CMS - Forget Password",
            PrimaryMessage = $"{baseUrl}/{encodedToken}/{user.Id}",
            EmailProcess = EmailProcess.ForgetPassword,
            UserName = user.UserName ?? "",
        };

        await emailService.SendEmail(emailDto);
    }

    public async Task ResetPassword(ResetPasswordRequestDto forgotPasswordDto)
    {
        var user = genericRepository.GetById<User>(forgotPasswordDto.UserId) ??
                   throw new NotFoundException("The following user has not been registered to our system.");
        
        var emailDto = new EmailDto
        {
            FullName = user.Name,
            ToEmailAddress = user.Email ?? "",
            Subject = "Connect CMS - Reset Password",
            EmailProcess = EmailProcess.ResetPassword,
            UserName = user.UserName ?? "",
            Password = forgotPasswordDto.Password
        };

        await emailService.SendEmail(emailDto);
    }

    public async Task TrainingRequest(TrainingRequestsRequestDto trainingRequestsRequest)
    {
        var candidate = genericRepository.GetById<User>(currentUserService.GetUserId)
                        ?? throw new NotFoundException("The following user has not been registered to our system.");
        
        var training = genericRepository.GetById<Training>(trainingRequestsRequest.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");
        
        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x => x.TrainingId == training.Id && x.CandidateId == candidate.Id)
                                ?? throw new NotFoundException("The following training candidate could not be found.");
        
        var emailDto = new EmailDto
        {
            FullName = candidate.Name,
            ToEmailAddress = candidate.Email ?? "",
            Subject = "Connect CMS - Training Request",
            EmailProcess = EmailProcess.TrainingRequest,
            UserName = candidate.UserName ?? "",
            PrimaryMessage = trainingCandidate.RequestedDate.ToFormattedDateTime(),
            SecondaryMessage = $"<b>{training.Title}</b>"
        };

        await emailService.SendEmail(emailDto);
    }

    public async Task TrainingRequestAction(TrainingRequestsActionRequestDto trainingRequestsActionRequest)
    {
        var training = genericRepository.GetById<Training>(trainingRequestsActionRequest.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        foreach (var requestAction in trainingRequestsActionRequest.RequestActions)
        {
            var user = genericRepository.GetById<User>(requestAction.UserId)
                       ?? throw new NotFoundException("The following user has not been registered to our system.");

            if (requestAction.IsApproved)
            {
                var emailDto = new EmailDto
                {
                    FullName = user.Name,
                    ToEmailAddress = user.Email ?? "",
                    Subject = "Connect CMS - Training Request Approved",
                    EmailProcess = EmailProcess.TrainingApprovedRequest,
                    UserName = user.UserName ?? "",
                    PrimaryMessage = $"{training.Title}"
                };

                await emailService.SendEmail(emailDto);
            }
            else
            {
                var emailDto = new EmailDto
                {
                    FullName = user.Name,
                    ToEmailAddress = user.Email ?? "",
                    Subject = "Connect CMS - Training Request Rejected",
                    EmailProcess = EmailProcess.TrainingRejectedRequest,
                    UserName = user.UserName ?? "",
                    PrimaryMessage = $"{training.Title}",
                    SecondaryMessage = requestAction.Remarks
                };

                await emailService.SendEmail(emailDto);
            }
        }
    }
}
