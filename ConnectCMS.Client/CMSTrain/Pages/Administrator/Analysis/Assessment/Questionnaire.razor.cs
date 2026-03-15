using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.Administrator.Analysis.Assessment;

public partial class Questionnaire
{
    [Parameter] public Guid InspectionId { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.PersonalAssessmentsQuestionnaire;
    }
    #endregion
}