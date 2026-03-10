using CMSTrain.Client.Layout.Application;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.PersonalityTest;

namespace CMSTrain.Client.Pages.Candidate.PersonalityTest;

public partial class PersonalityTestAnalytics
{
    [Parameter] public Guid UserResponseId { get; set; }

    private bool IsOverviewSelected { get; set; } = true;

    private bool IsRendered { get; set; } = true;
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetPersonalityTestAnalysis();
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.PersonalityTestAnalytics;
    }
    #endregion
    
    private GetPersonalityTestAnalysisDto PersonalityTestAnalysisModel { get; set; } = new();
    
    private async Task GetPersonalityTestAnalysis()
    {
        try
        {
            var result = await PersonalityTestService.GetPersonalityTestAnalysis(UserResponseId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return;
            }

            PersonalityTestAnalysisModel = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private PersonalityTestAnalysis? SelectedTraitAnalysis { get; set; }
    
    private PersonalityTestAnalysis? SelectedFacet { get; set; }

    private void SelectTrait(string trait)
    {
        IsOverviewSelected = false;

        SelectedTraitAnalysis = PersonalityTestAnalysisModel?.Analyses.FirstOrDefault(a => a.Trait == trait);
        
        SelectedFacet = null;
    }

    private void SelectFacet(PersonalityTestAnalysis facet)
    {
        IsOverviewSelected = false;
        
        SelectedFacet = facet;
    }
    
    private void SelectOverview()
    {
        IsOverviewSelected = true;

        SelectedTraitAnalysis = null;
        
        SelectedFacet = null;
    }
}