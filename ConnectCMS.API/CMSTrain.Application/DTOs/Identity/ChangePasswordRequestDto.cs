namespace CMSTrain.Application.DTOs.Identity;

public class ChangePasswordRequestDto
{
    public string CurrentPassword { get; set; }

    public string NewPassword { get; set; }

    public string ConfirmPassword { get; set;}
}
