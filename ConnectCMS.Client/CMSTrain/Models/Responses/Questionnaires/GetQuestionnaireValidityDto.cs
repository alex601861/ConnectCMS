namespace CMSTrain.Client.Models.Responses.Questionnaires;

public class GetQuestionnaireValidityDto
{
    public bool IsValid { get; set; }
    
    public bool IsAnswered { get; set; }
}