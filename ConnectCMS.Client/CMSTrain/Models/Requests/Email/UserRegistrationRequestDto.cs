namespace CMSTrain.Client.Models.Requests.Email;

public  class UserRegistrationRequestDto : RegistrationEmailRequestDto
{
    public string Password { get; set; } = string.Empty;
}
