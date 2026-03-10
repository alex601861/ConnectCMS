using CMSTrain.Client.Layout.Application;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Strategy;
using CMSTrain.Client.Models.Responses.Questionnaires;
using CMSTrain.Client.Models.Responses.Strategy;
using CMSTrain.Client.Models.Responses.Training;

namespace CMSTrain.Client.Pages.Candidate.Strategy;

public partial class StrategicTraitQuestionnaire : ComponentBase
{
    private bool IsLoading { get; set; } = true;
    
    [Parameter] public Guid QuestionnaireId { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        await GetQuestionnaireValidity();

        if (QuestionnaireValidity is { IsValid: true, IsAnswered: false })
        {
            await GetStrategicTraitModuleDetails();
        }

        IsLoading = false;
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.StrategicTraitQuestionnaire;
    }
    #endregion

    #region Questionnaire Validity
    private GetQuestionnaireValidityDto QuestionnaireValidity { get; set; } = new();
    
    private async Task GetQuestionnaireValidity()
    {
        try
        {
            var result = await QuestionnaireService.GetQuestionnaireValidity(QuestionnaireId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            QuestionnaireValidity = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Strengths and Weaknesses
    private List<GetStrategyModuleDto> Strengths { get; set; } = [];
    
    private List<GetStrategyModuleDto> Weaknesses { get; set; } = [];

    private async Task GetStrategicTraitModuleDetails()
    {
        try
        {
            var strengths = await StrategicTraitService.GetAllStrategyModules(StrategicType.Strength);

            if (strengths?.Result is null)
            {
                SnackbarService.ShowSnackbar(strengths?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Strengths = strengths.Result;
         
            var weaknesses = await StrategicTraitService.GetAllStrategyModules(StrategicType.Weakness);

            if (weaknesses?.Result is null)
            {
                SnackbarService.ShowSnackbar(weaknesses?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Weaknesses = weaknesses.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region SWOT Analysis Form Submission
    private List<Guid> SelectedStrengthIds { get; set; } = [];

    private List<Guid> SelectedWeaknessIds { get; set; } = [];

    private bool _isDisabled;

    private bool IsDisabled
    {
        get => _isDisabled ||
               SelectedStrengthIds.Count is < 3 or > 8 || 
               SelectedWeaknessIds.Count is < 3 or > 8;
        set => _isDisabled = value;
    }
    
    private void OnSelectStrategicTrait((Guid Id, bool IsSelected, StrategicType Type) result)
    {
        if (result.Type == StrategicType.Strength)
        {
            if (result.IsSelected)
            {
                SelectedStrengthIds.Add(result.Id);
            }
            else
            {
                SelectedStrengthIds.Remove(result.Id);
            }
        }
        else
        {
            if (result.IsSelected)
            {
                SelectedWeaknessIds.Add(result.Id);
            }
            else
            {
                SelectedWeaknessIds.Remove(result.Id);
            }
        }

        StateHasChanged();
    }

    private async Task UploadStrategicTraits()
    {
        try
        {
            IsDisabled = true;

            var strategicTraits = new UploadStrategyTraitQuestionnaireDto()
            {
                QuestionnaireId = QuestionnaireId,
                StrengthIds = SelectedStrengthIds,
                WeaknessIds = SelectedWeaknessIds
            };

            var result = await StrategicTraitService.UploadStrategyTraitQuestionnaire(strategicTraits);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    var training = await GetTrainingDetailsByQuestionnaire();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    NavigationManager.NavigateTo($"/candidate/assigned-trainings/training-details/{training.Id}");
                    break;
                case StatusCode.Status404NotFound:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        finally
        {
            IsDisabled = false;
        }
    }
    #endregion

    #region Training Details Navigation
    private async Task<GetTrainingDto> GetTrainingDetailsByQuestionnaire()
    {
        var result = await TrainingService.GetTrainingDetailsByQuestionnaire(QuestionnaireId);

        if (result?.Result is null)
        {
            SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);

            return new GetTrainingDto();
        }

        var training = result.Result;
        
        return training;
    }
    #endregion
}