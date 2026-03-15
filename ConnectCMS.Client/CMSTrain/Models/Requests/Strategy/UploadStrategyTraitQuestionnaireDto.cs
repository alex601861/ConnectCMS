namespace CMSTrain.Client.Models.Requests.Strategy;

public class UploadStrategyTraitQuestionnaireDto
{
    public Guid QuestionnaireId { get; set; }
    
    public List<Guid> StrengthIds { get; set; }
    
    public List<Guid> WeaknessIds { get; set; }
}