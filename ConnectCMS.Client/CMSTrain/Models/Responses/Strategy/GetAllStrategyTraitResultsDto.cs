namespace CMSTrain.Client.Models.Responses.Strategy;

public class GetAllStrategyTraitResultsDto
{
    public List<GetStrategyModuleDto> Opportunities { get; set; } = [];

    public List<GetStrategyModuleDto> Threats { get; set; } = [];
}