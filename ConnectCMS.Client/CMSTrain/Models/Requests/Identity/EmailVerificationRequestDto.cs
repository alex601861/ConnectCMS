namespace CMSTrain.Client.Models.Requests.Identity;

public class EmailVerificationRequestDto
{
    public string UserId { get; set; }

    public string VerificationCode { get; set; }    
}
