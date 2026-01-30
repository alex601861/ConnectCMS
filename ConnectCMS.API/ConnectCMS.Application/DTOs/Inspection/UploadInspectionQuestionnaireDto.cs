using CMSTrain.Application.DTOs.Questionnaires;

namespace CMSTrain.Application.DTOs.Inspection;

public class UploadInspectionQuestionnaireDto
{
    public Guid InspectionId { get; set; }
    
    public bool RequiresPredefinedAnswers { get; set; }

    public List<McqAnswerDetailsDto>? Answers { get; set; }
    
    public List<QuestionDetailsDto> QuestionnaireDetails { get; set; }
}