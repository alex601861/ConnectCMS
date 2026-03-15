namespace CMSTrain.Client.Models.Responses.Questionnaires;

public class GetHeadingQuestionDetailsDto : HeadingBaseDto
{
    public List<SubHeadingDto> SubHeadingQuestions { get; set; } = [];

    public List<GetQuestionDetailsDto> Questions { get; set; } = [];
}

public class HeadingBaseDto
{
    public Guid HeadingId { get; set; }
    
    public string Heading { get; set; } = string.Empty;
}

public class SubHeadingDto : HeadingBaseDto
{
    public List<GetQuestionDetailsDto> Questions { get; set; } = [];
}