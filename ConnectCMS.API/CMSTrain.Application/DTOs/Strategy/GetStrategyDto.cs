namespace CMSTrain.Application.DTOs.Strategy;

// Parent Strategy Traits
public class GetStrategyDto : GetStrategyModuleDto
{
    public List<GetStrategyModuleDto> Opportunities { get; set; }

    public List<GetStrategyModuleDto> Threats { get; set; }
}