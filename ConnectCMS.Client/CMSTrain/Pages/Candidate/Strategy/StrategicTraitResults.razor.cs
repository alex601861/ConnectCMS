using CMSTrain.Client.Models.Responses.Strategy;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.Candidate.Strategy;

public partial class StrategicTraitResults
{
    [Parameter] public string Title { get; set; } = string.Empty;
    
    [Parameter] public List<GetStrategyModuleDto> Traits { get; set; } = new();
}