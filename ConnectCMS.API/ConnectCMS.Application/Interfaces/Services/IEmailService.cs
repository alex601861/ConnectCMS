using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Email;
using CMSTrain.Application.DTOs.Identity;

namespace CMSTrain.Application.Interfaces.Services;

public interface IEmailService : ITransientService
{
    Task SendEmail(EmailDto emailDto);
}
