namespace CMSTrain.Client.Models.Base;

public class GlobalState
{
    public Guid UserId { get; set; }
    
    public string Name { get; set; } = "";

    public string EmailAddress { get; set; } = "";
    
    public Guid RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? ImageUrl { get; set; } = "";
}