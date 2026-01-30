namespace CMSTrain.Application.DTOs.EmailConfirmation;

public  class UserRegistrationRequestDto : RegistrationEmailRequestDto
{
    public string Password { get; set; }
}
