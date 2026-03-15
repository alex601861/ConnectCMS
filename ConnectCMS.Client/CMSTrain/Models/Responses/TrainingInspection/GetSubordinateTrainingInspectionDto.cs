using CMSTrain.Client.Models.Responses.Inspection;

namespace CMSTrain.Client.Models.Responses.TrainingInspection;

public class GetSubordinateTrainingInspectionDto : GetInspectionDto
{
    public List<SubordinateQuestionnaireResponseDto> QuestionnaireResponses { get; set; } = [];
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