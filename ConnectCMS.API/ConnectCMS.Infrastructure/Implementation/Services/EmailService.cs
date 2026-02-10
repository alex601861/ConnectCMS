using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Common.Enum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using CMSTrain.Application.Settings;
using CMSTrain.Application.DTOs.Email;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class EmailService(IOptions<MailSettings> smtpSettings, IWebHostEnvironment webHostEnvironment) : IEmailService
{
    private readonly MailSettings _mailSettings = smtpSettings.Value;

    private const string EmailPath = Constants.FilePath.EmailTemplateFilePath;

    public async Task SendEmail(EmailDto emailDto)
    {
        try
        {
            using var emailMessage = new MimeMessage();

            #region Senders and Receivers
            var emailFrom = new MailboxAddress("Connect CMS", _mailSettings.Username);
            var emailTo = new MailboxAddress(emailDto.FullName, emailDto.ToEmailAddress);
            
            var emailBccAffinity = new MailboxAddress(Constants.SuperAdmin.MailSettings.Username, Constants.SuperAdmin.MailSettings.EmailAddress);
            var emailBccCmsConnect = new MailboxAddress("Connect CMS", _mailSettings.Username);

            emailMessage.From.Add(emailFrom);
            emailMessage.To.Add(emailTo);
            
            emailMessage.Bcc.Add(emailBccAffinity);
            emailMessage.Bcc.Add(emailBccCmsConnect);
            #endregion

            #region Mail Content and Details
            emailMessage.Subject = emailDto.Subject;
            
            emailDto.PlaceHolders = GetPlaceHolders(emailDto);
            emailDto.Body = PrepareTemplate(emailDto);
            
            var emailBodyBuilder = new BodyBuilder()
            {
                HtmlBody = emailDto.Body
            };
            
            emailMessage.Body = emailBodyBuilder.ToMessageBody();
            #endregion

            #region Fire and Trigger Mail
            using var mailClient = new SmtpClient();
            
            await mailClient.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await mailClient.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
            await mailClient.SendAsync(emailMessage);
            await mailClient.DisconnectAsync(true);
            #endregion
        }
        catch (Exception ex)
        {
            throw new BadRequestException("An email could not be triggered to your respective email address.", [ex.Message]);
        }
    }
    
    private static List<KeyValuePair<string, string>> GetPlaceHolders(EmailDto emailDto)
    {
        var result = new List<KeyValuePair<string, string>>();
        
        switch (emailDto.EmailProcess)
        {
            case EmailProcess.SelfRegistration:
                result.Add(new KeyValuePair<string, string>("{$fullName}", emailDto.FullName));
                result.Add(new KeyValuePair<string, string>("{$userName}", emailDto.UserName ?? ""));
                result.Add(new KeyValuePair<string, string>("{$primaryMessage}", emailDto.PrimaryMessage));
                break;
            case EmailProcess.UserRegistration:
                result.Add(new KeyValuePair<string, string>("{$fullName}", emailDto.FullName));
                result.Add(new KeyValuePair<string, string>("{$role}", emailDto.Role ?? ""));
                result.Add(new KeyValuePair<string, string>("{$userName}", emailDto.UserName ?? ""));
                result.Add(new KeyValuePair<string, string>("{$password}", emailDto.Password ?? ""));
                result.Add(new KeyValuePair<string, string>("{$primaryMessage}", emailDto.PrimaryMessage));
                break;
            case EmailProcess.ClientCandidateRegistration:
                result.Add(new KeyValuePair<string, string>("{$fullName}", emailDto.FullName));
                result.Add(new KeyValuePair<string, string>("{$role}", emailDto.Role ?? ""));
                result.Add(new KeyValuePair<string, string>("{$userName}", emailDto.UserName ?? ""));
                result.Add(new KeyValuePair<string, string>("{$password}", emailDto.Password ?? ""));
                result.Add(new KeyValuePair<string, string>("{$primaryMessage}", emailDto.PrimaryMessage));
                break;
            case EmailProcess.ForgetPassword:
                result.Add(new KeyValuePair<string, string>("{$fullName}", emailDto.FullName));
                result.Add(new KeyValuePair<string, string>("{$userName}", emailDto.UserName ?? ""));
                result.Add(new KeyValuePair<string, string>("{$primaryMessage}", emailDto.PrimaryMessage));
                break;
            case EmailProcess.ResetPassword:
                result.Add(new KeyValuePair<string, string>("{$fullName}", emailDto.FullName));
                result.Add(new KeyValuePair<string, string>("{$userName}", emailDto.UserName ?? ""));
                result.Add(new KeyValuePair<string, string>("{$password}", emailDto.Password ?? ""));
                break;
            case EmailProcess.TrainingRequest:
                result.Add(new KeyValuePair<string, string>("{$fullName}", emailDto.FullName));
                result.Add(new KeyValuePair<string, string>("{$primaryMessage}", emailDto.PrimaryMessage));
                break;
            case EmailProcess.TrainingApprovedRequest:
                result.Add(new KeyValuePair<string, string>("{$fullName}", emailDto.FullName));
                result.Add(new KeyValuePair<string, string>("{$primaryMessage}", emailDto.PrimaryMessage));
                break;
            case EmailProcess.TrainingRejectedRequest:
                result.Add(new KeyValuePair<string, string>("{$fullName}", emailDto.FullName));
                result.Add(new KeyValuePair<string, string>("{$primaryMessage}", emailDto.PrimaryMessage));
                result.Add(new KeyValuePair<string, string>("{$remarks}", emailDto.SecondaryMessage ?? ""));
                break;
            case EmailProcess.SubordinatesQuestionnaire:
                result.Add(new KeyValuePair<string, string>("{$fullName}", emailDto.FullName));
                result.Add(new KeyValuePair<string, string>("{$userName}", emailDto.UserName ?? ""));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return result;
    }
    
    private static string UpdatePlaceHolders(string text, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
    {
        if (string.IsNullOrEmpty(text)) return text;
        
        foreach (var placeholder in keyValuePairs.Where(placeholder => text.Contains(placeholder.Key)))
        {
            text = text.Replace(placeholder.Key, placeholder.Value);
        }

        return text;
    }

    private string PrepareTemplate(EmailDto emailDto)
    {
        return UpdatePlaceHolders(GetEmailBody(emailDto.EmailProcess.ToString()), emailDto.PlaceHolders);
    }

    private string GetEmailBody(string templateName)
    {           
        return File.ReadAllText(Path.Combine(webHostEnvironment.WebRootPath, EmailPath, $"{templateName}.html"));
    }
}
