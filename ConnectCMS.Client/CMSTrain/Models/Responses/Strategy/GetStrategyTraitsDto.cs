namespace CMSTrain.Client.Models.Responses.Strategy;

public class GetStrategyTraitsDto : GetAllStrategyTraitResultsDto
{
    public List<GetStrategyModuleDto> Strengths { get; set; } = [];

    public List<GetStrategyModuleDto> Weaknesses { get; set; } = [];
}