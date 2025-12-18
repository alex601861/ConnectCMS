namespace CMSTrain.Application.DTOs.Identity;

public class EmailVerificationRequestDto
{
    public string UserId { get; set; }

    public string VerificationCode { get; set; }    
}
