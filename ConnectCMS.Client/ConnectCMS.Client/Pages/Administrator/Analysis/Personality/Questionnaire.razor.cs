using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.Administrator.Analysis.Personality;

public partial class Questionnaire
{
    [Parameter] public Guid InspectionId { get; set; }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
    }

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.PersonalityTestQuestionnaire;
    }
    #endregion
}