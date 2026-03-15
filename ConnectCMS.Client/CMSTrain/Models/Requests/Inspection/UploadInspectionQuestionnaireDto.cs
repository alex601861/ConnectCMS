using CMSTrain.Client.Models.Requests.Questionnaires;

namespace CMSTrain.Client.Models.Requests.Inspection;

public class UploadInspectionQuestionnaireDto
{
    public Guid InspectionId { get; set; }
    
    public bool RequiresPredefinedAnswers { get; set; }
    
    public List<McqAnswerDetailsDto>? Answers { get; set; } = [];

    public List<QuestionDetailsDto> QuestionnaireDetails { get; set; } = [];
}