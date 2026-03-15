using CMSTrain.Client.Layout.Application;
using MudBlazor;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.PersonalityTest;

namespace CMSTrain.Client.Pages.Candidate.PersonalityTest;

public partial class PersonalityTestScores
{
    [Parameter] public Guid UserResponseId { get; set; }
    
    private int ActivePanelIndex { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        
        await GetPersonalityTestResponses();
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.PersonalityTestScore;
    }
    #endregion

    private GetPersonalityTestResponseDto PersonalityTestResponse { get; set; } = new();

    private async Task GetPersonalityTestResponses()
    {
        try
        {
            var result = await PersonalityTestService.GetPersonalityTestResponses(UserResponseId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return;
            }

            PersonalityTestResponse = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private async Task NavigateTabs(bool isNext)
    {
        if (isNext)
        {
            ActivePanelIndex++;
        }
        else
        {
            ActivePanelIndex--;
        }

        await ScrollManager.ScrollToAsync(null, 0, 0, ScrollBehavior.Smooth);
    }
    
    private void NavigateToPersonalityTestAnalysis()
    {
        NavigationManager.NavigateTo($"/personality-test-analysis/{UserResponseId}");
    }
}