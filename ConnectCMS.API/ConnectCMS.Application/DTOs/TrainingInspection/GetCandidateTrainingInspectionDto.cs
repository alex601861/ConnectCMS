using CMSTrain.Application.DTOs.Inspection;

namespace CMSTrain.Application.DTOs.TrainingInspection;

public class GetCandidateTrainingInspectionDto : GetInspectionDto
{
    public List<QuestionnaireResponseDto> QuestionnaireResponses { get; } = [];
}

public class QuestionnaireResponseDto
{
    public Guid QuestionnaireId { get; set; }
    
    public bool IsEligible { get; set; }

    public Guid? UserResponseId { get; set; }
    
    public string? AnsweredDate { get; set; }

    public string EligibilityPeriod { get; set; } = string.Empty;
}