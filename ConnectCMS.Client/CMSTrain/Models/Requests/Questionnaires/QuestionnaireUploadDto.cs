namespace CMSTrain.Client.Models.Requests.Questionnaires;

public class QuestionnaireUploadDto : QuestionnaireDto
{
    public List<QuestionDetailsDto> QuestionDetails { get; set; } = [];
}