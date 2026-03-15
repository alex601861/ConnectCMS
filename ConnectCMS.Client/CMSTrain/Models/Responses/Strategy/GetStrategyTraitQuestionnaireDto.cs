namespace CMSTrain.Client.Models.Responses.Strategy;

public class GetStrategyTraitQuestionnaireDto
{
    public Guid Id { get; set; }
    
    public string AnsweredDate { get; set; }
    
    public int Strengths { get; set; }
    
    public int Weaknesses { get; set; }
    
    public int Opportunities { get; set; }
    
    public int Threats { get; set; }
}