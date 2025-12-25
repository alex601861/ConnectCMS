namespace CMSTrain.Client.Models.Requests.Identity;

public class ResetUserPasswordRequestDto
{
    public Guid UserId { get; set; }
    
    public string Password { get; set; }
    
    public string EmailAddress { get; set; }
}