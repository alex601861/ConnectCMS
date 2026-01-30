namespace CMSTrain.Application.DTOs.EmailConfirmation;

public class ResetPasswordRequestDto
{
    public Guid UserId { get; set; }
    
    public string Password { get; set; }
}