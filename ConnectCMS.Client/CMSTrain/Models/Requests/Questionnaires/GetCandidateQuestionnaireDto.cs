using CMSTrain.Client.Models.Responses.Questionnaires;

namespace CMSTrain.Client.Models.Requests.Questionnaires;

public class GetCandidateQuestionnaireDto
{
    public Guid QuestionnaireId { get; set; }

    public Guid TrainingInspectionId { get; set; }
    
    public List<GetHeadingQuestionDetailsDto> HeadingQuestions { get; set; } = [];
    
    public List<GetQuestionDetailsDto> Questions { get; set; } = [];
}