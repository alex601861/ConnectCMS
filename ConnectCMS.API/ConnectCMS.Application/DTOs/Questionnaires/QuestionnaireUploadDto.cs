namespace CMSTrain.Application.DTOs.Questionnaires;

public class QuestionnaireUploadDto : QuestionnaireDto
{
    public List<QuestionDetailsDto> QuestionDetails { get; set; } = [];
}