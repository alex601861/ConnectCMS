namespace CMSTrain.Client.Models.Requests.Email;

public class ResetPasswordRequestDto
{
    public Guid UserId { get; set; }

    public string Password { get; set; } = string.Empty;
}