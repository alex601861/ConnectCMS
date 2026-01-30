using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Application.DTOs.Email;

public class EmailDto
{
    #region Required Fields
    public string FullName { get; set; } = "";
    
    public string ToEmailAddress { get; set; } = "";

    public string Subject { get; set; } = "";

    public string PrimaryMessage { get; set; } = "";

    public EmailProcess EmailProcess { get; set; }
    #endregion

    #region Calculated Fields
    public string Body { get; set; } = "";

    public List<KeyValuePair<string, string>> PlaceHolders { get; set; } = [];
    #endregion
    
    #region Optional Fields
    public string? UserName { get; set; }
    
    public string? Password { get; set; }       

    public string? Role { get; set; }       
    
    public string? Cc { get; set; }
    
    public string? SecondaryMessage { get; set; }

    public string? TertiaryMessage { get; set; }

    public string? Remarks { get; set; }   
    #endregion
}