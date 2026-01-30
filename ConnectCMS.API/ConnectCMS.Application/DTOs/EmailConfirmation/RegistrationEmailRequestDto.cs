namespace CMSTrain.Application.DTOs.EmailConfirmation;

public class RegistrationEmailRequestDto
{
    public string? UserId { get; set; }

    public string? EmailAddress { get; set; }
}