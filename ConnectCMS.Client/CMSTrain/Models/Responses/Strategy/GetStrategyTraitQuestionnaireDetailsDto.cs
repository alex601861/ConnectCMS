namespace CMSTrain.Client.Models.Responses.Strategy;

public class GetStrategyTraitQuestionnaireDetailsDto
{
    public GetStrategyTraitQuestionnaireDto Questionnaire { get; set; } = new();
    
    public List<Traits> Strengths { get; set; } = [];
    
    public List<Traits> Weaknesses { get; set; } = [];
    
    public List<GetStrategyModuleDto> Opportunities { get; set; } = [];
    
    public List<GetStrategyModuleDto> Threats { get; set; } = [];
    
    public Guid QuestionnaireId { get; set; }
    
    public Guid TrainingId { get; set; }
}

public class Traits
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Type { get; set; }
    
    public bool IsSelected { get; set; }
}