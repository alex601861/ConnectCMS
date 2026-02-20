namespace CMSTrain.Application.DTOs.Identity;

public class ForgotPasswordEmailRequestDto
{
    public string UserId { get; set; }

    public string Token { get; set; }

    public string NewPassword {  get; set; }
}
