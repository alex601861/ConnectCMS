using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Strategy;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Candidate.Strategy;

public partial class StrategicTraitResponses : ComponentBase
{
    [Parameter]
    public Guid ResponseId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        
        await GetAllStrategicTraitResponses();

        await GetNavigationDetails();

        GetStrategyTraitsResult();
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.StrategicTraitResponses;
    }
    #endregion

    #region Responses
    private string StrengthCounts => "You have selected " + StrategicTraitResponse.Strengths.Count(x => x.IsSelected) + " out of 7 strengths.";

    private string WeaknessCounts => "You have selected " + StrategicTraitResponse.Strengths.Count(x => x.IsSelected) + " out of 7 weaknesses.";
    
    private GetStrategyTraitQuestionnaireDetailsDto StrategicTraitResponse { get; set; } = new();
    
    private async Task GetAllStrategicTraitResponses()
    {
        try
        {
            var result = await StrategicTraitService.GetStrategyTraitQuestionnaireDetails(ResponseId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            StrategicTraitResponse = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Results and Analysis
    private GetStrategyTraitsDto StrategyTraitsResult { get; set; } = new();
    
    private void GetStrategyTraitsResult()
    {
        var strengths = StrategicTraitResponse.Strengths.Where(x => x.IsSelected).Select(x => new GetStrategyModuleDto()
        {
            Id = x.Id,
            Name = x.Name,
            Type = x.Type,
            Description = x.Description,
        }).ToList();

        var weaknesses = StrategicTraitResponse.Weaknesses.Where(x => x.IsSelected).Select(x =>
            new GetStrategyModuleDto()
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Description = x.Description,
            }).ToList();
        
        StrategyTraitsResult = new GetStrategyTraitsDto
        {
            Strengths = strengths,
            Weaknesses = weaknesses,
            Opportunities = StrategicTraitResponse.Opportunities,
            Threats = StrategicTraitResponse.Threats
        };
    }
    #endregion
    
    #region Role Access & Details
    private string StrategicTraitResponseNavigation { get; set; } = string.Empty;

    private async Task GetNavigationDetails()
    {
        try
        {
            var response = await ProfileService.GetUserRole();

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            var role = response.Result;

            StrategicTraitResponseNavigation = role.Name switch
            {
                Constants.Roles.SuperAdmin =>
                    $"/trainings/admin/training-details/{StrategicTraitResponse.QuestionnaireId}/5",
                Constants.Roles.Admin =>
                    $"/trainings/admin/training-details/{StrategicTraitResponse.QuestionnaireId}/5",
                Constants.Roles.Client =>
                    $"/client/assigned-trainings/training-details/{StrategicTraitResponse.TrainingId}/5",
                Constants.Roles.Trainer =>
                    $"/trainer/assigned-trainings/training-details/{StrategicTraitResponse.TrainingId}/5",
                Constants.Roles.Candidate =>
                    $"/candidate/assigned-trainings/training-details/{StrategicTraitResponse.TrainingId}/5",
                _ => StrategicTraitResponseNavigation
            };
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
}