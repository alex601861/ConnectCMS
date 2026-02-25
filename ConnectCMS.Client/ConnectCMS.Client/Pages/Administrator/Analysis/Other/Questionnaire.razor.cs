using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.Administrator.Analysis.Other;

public partial class Questionnaire
{
    [Parameter] public Guid InspectionId { get; set; }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();
    
    protected override Task OnInitializedAsync()
    {
        SetPageTitle();
        
        return Task.CompletedTask;
    }

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.FeedbacksQuestionnaire;
    }
    #endregion
}