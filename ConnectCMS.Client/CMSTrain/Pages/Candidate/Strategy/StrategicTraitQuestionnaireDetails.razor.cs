using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Strategy;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.Candidate.Strategy;

public partial class StrategicTraitQuestionnaireDetails : ComponentBase
{
    [Parameter] public string Title { get; set; }
    
    [Parameter] public StrategicType StrategicType { get; set; }
    
    [Parameter] public string Description { get; set; }
    
    [Parameter] public string Icon { get; set; }
    
    [Parameter] public List<GetStrategyModuleDto> Traits { get; set; } = new();
    
    [Parameter] public List<Guid> SelectedTraitIds { get; set; } = new();
    
    [Parameter] public EventCallback<(Guid Id, bool IsSelected, StrategicType type)> OnSelectStrategicTrait { get; set; }
    
    [Parameter] public StrategicType Type { get; set; }
    
    private void SelectTrait(Guid strengthId, bool isSelected, StrategicType type)
    {
        OnSelectStrategicTrait.InvokeAsync((strengthId, isSelected, type));
    }
}