namespace CMSTrain.Client.Models.Responses.Answers;

public class GetAnswerDetailsDto
{
    public Guid Id { get; set; }
    
    public Guid QuestionnaireId { get; set; }
    
    public Guid TrainingInspectionId { get; set; }
    
    public Guid CandidateId { get; set; }

    public Guid? SubordinateId { get; set; }
    
    public string AnsweredDate { get; set; }
    
    public bool IsAnsweredByCandidate { get; set; }

    public bool IsAnsweredBySubordinate { get; set; }
    
    public int Phase { get; set; }

    public string? Remarks { get; set; }

    public GetResponseAnalysisDto? Analysis { get; set; }

    public List<GetQuestionAnswerDetailsDto> QuestionAnswers { get; set; } = [];
}

