namespace CMSTrain.Application.DTOs.Analysis;

public class InspectionResponseAnalysisDto
{
    public Guid QuestionId { get; set; }
    
    public string QuestionType { get; set; }
    
    public List<ResponseAnalysis> Responses { get; set; }
}

public class ResponseAnalysis
{
    public string Respondent { get; set; }

    public double Score { get; set; }

    public string Responses { get; set; }
}