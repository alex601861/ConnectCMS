namespace CMSTrain.Client.Models.Responses.Analysis;

public class GetAssessmentResponseAnalysisDto
{
    public Guid QuestionId { get; set; }
    
    public string Question { get; set; }
    
    public string QuestionType { get; set; }
    
    public List<ResponseAnalysis> Responses { get; set; }
}

public class ResponseAnalysis
{
    public string Respondent { get; set; }

    public double Score { get; set; }

    public string Responses { get; set; }
}