using CMSTrain.Client.Models.Responses.Strategy;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.Candidate.Strategy;

public partial class StrategicTraitAnalysis
{
    [Parameter] public GetStrategyTraitsDto StrategyTraitsResult { get; set; } = new();
}