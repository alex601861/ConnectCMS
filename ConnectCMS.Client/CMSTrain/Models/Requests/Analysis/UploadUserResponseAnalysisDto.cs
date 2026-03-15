namespace CMSTrain.Client.Models.Requests.Analysis;

public class UploadUserResponseAnalysisDto
{
    public Guid UserResponseId { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string Score { get; set; }
}