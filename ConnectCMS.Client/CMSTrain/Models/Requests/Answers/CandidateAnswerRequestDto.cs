namespace CMSTrain.Client.Models.Requests.Answers;

public class CandidateAnswerRequestDto
{
    public Guid QuestionnaireId { get; set; }

    public string Remarks { get; set; }
    
    public List<AnswerDetailDto> Answers { get; set; }
}
