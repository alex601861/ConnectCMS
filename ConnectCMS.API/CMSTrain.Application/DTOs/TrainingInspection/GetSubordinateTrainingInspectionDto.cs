using CMSTrain.Application.DTOs.Inspection;

namespace CMSTrain.Application.DTOs.TrainingInspection;

public class GetSubordinateTrainingInspectionDto : GetInspectionDto
{
    public List<SubordinateQuestionnaireResponseDto> QuestionnaireResponses { get; } = [];
}

public class SubordinateQuestionnaireResponseDto
{
    public Guid SubordinateId { get; set; }
    
    public Guid QuestionnaireId { get; set; }
    
    public bool IsEligible { get; set; }

    public Guid? UserResponseId { get; set; }
    
    public string? AnsweredDate { get; set; }

    public string EligibilityPeriod { get; set; } = string.Empty;
}