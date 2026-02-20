namespace CMSTrain.Application.DTOs.Strategy;

public class GetAllStrategyTraitResultsDto
{
    public List<GetStrategyModuleDto> Opportunities { get; set; }

    public List<GetStrategyModuleDto> Threats { get; set; }
}