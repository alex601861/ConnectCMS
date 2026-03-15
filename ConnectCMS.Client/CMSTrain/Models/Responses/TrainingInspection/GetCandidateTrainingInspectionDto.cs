using CMSTrain.Client.Models.Responses.Inspection;

namespace CMSTrain.Client.Models.Responses.TrainingInspection;

public class GetCandidateTrainingInspectionDto : GetInspectionDto
{
    public List<QuestionnaireResponseDto> QuestionnaireResponses { get; set; } = [];
}

public class QuestionnaireResponseDto
{
    public Guid QuestionnaireId { get; set; }
    
    public bool IsEligible { get; set; }

    public Guid? UserResponseId { get; set; }
    
    public string? AnsweredDate { get; set; }
    
    public string EligibilityPeriod { get; set; }
}