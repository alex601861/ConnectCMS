namespace CMSTrain.Application.DTOs.Analysis;

public class GetAssessmentResponseAnalysisDto
{
    public Guid QuestionId { get; set; }
    
    public string Question { get; set; }
    
    public string QuestionType { get; set; }
    
    public List<ResponseAnalysis> Responses { get; set; }
}