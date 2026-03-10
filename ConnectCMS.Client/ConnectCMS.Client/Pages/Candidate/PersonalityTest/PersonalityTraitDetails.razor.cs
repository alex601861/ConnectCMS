using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.PersonalityTrait;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Candidate.PersonalityTest;

public partial class PersonalityTraitDetails
{
    [Parameter] public string Trait { get; set; } = string.Empty;
    
    private TraitType TraitType { get; set; } = TraitType.Openness;

    protected override async Task OnInitializedAsync()
    {
        TraitType = Trait switch
        {
            "Openness" => TraitType.Openness,
            "Conscientiousness" => TraitType.Conscientiousness,
            "Extraversion" => TraitType.Extraversion,
            "Agreeableness" => TraitType.Agreeableness,
            "Neuroticism" => TraitType.Neuroticism,
            _ => TraitType.Openness
        };

        await GetPersonalityTraitDetails();
    }

    private GetPersonalityTraitDto PersonalityTrait { get; set; } = new();
    
    private async Task GetPersonalityTraitDetails()
    {
        try
        {
            var result = await PersonalityTraitService.GetPersonalityTrait(TraitType);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                
                return;
            }

            PersonalityTrait = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
}