namespace CMSTrain.Client.Models.Responses.Strategy;

// Child Strategy Traits
public class GetStrategyDetailsDto
{
    public List<GetStrategyModuleDto> Opportunities { get; set; } = [];
    
    public List<GetStrategyModuleDto> Threats { get; set; } = [];
}