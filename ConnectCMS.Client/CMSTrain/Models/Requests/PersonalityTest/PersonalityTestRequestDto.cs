namespace CMSTrain.Client.Models.Requests.PersonalityTest;

public class PersonalityTestRequestDto
{
    public Guid QuestionnaireId { get; set; }
    
    public List<PersonalityTestQuestionnaire> Answers { get; set; }
}

public class PersonalityTestQuestionnaire
{
    public Guid QuestionId { get; set; }
    
    public Guid AnswerId { get; set; }
}