using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Strategy;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.Candidate.Strategy;

public partial class StrategicTraitResponsesDetails
{
    [Parameter] public string Title { get; set; }
    
    [Parameter] public StrategicType StrategicType { get; set; }
    
    [Parameter] public string Description { get; set; }
    
    [Parameter] public string Icon { get; set; }
    
    [Parameter] public List<Traits> Traits { get; set; } = new();

    [Parameter] public StrategicType Type { get; set; }
}