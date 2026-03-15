using System.ComponentModel.DataAnnotations;

namespace CMSTrain.Client.Models.Requests.Identity;

public class ChangePasswordRequestDto
{
    [Required(ErrorMessage = "Please enter your current password.")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "Please enter your new password.")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Please confirm your new password.")]
    public string ConfirmPassword { get; set;}
}
