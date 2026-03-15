namespace CMSTrain.Client.Models.Responses.Analysis;

public class GetUserResponseAnalysisDto
{
    public Guid Id { get; set; }
    
    public Guid UserResponseId { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string Score { get; set; }
}