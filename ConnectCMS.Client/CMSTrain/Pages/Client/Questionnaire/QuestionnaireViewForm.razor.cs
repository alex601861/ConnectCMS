using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Responses.Inspection;
using CMSTrain.Client.Models.Responses.Questionnaires;

namespace CMSTrain.Client.Pages.Client.Questionnaire;

public partial class QuestionnaireViewForm : ComponentBase
{
    [Parameter] public Guid QuestionnaireId { get; set; }

    private int ActivePanelIndex { get; set; }

    private bool IsLoading { get; set; } = true;
    
    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        
        var questionnaireModule = await GetQuestionnaireModuleDetails();

        SetPageTitle();

        await GetTrainingDetails(questionnaireModule.TrainingId);

        await GetInspectionDetails(questionnaireModule.InspectionId);

        StateHasChanged();

        IsLoading = false;
    }
    
    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.QuestionnaireViewForm;
    }
    #endregion

    #region Questionnaire Details
    private GetQuestionnaireDetailsDto QuestionnaireDetails { get; set; } = new();

    private async Task<GetQuestionnaireDetailsDto> GetQuestionnaireModuleDetails()
    {
        try
        {
            var result = await QuestionnaireService.GetQuestionnaireModuleDetails(QuestionnaireId);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                
                return new GetQuestionnaireDetailsDto();
            }
            
            QuestionnaireDetails = result.Result;

            return QuestionnaireDetails;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);

            return new GetQuestionnaireDetailsDto();
        }
    }
    
    #endregion
    
    #region Training Details
    private GetTrainingDto Training { get; set; } = new();

    private async Task GetTrainingDetails(Guid trainingId)
    {
        var training = await TrainingService.GetTrainingById(trainingId);
            
        if (training?.Result is null)
        {
            SnackbarService.ShowSnackbar(training?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
            return;
        }

        Training = training.Result;
        
        Training.ImageUrl = string.IsNullOrEmpty(Training.ImageUrl) 
            ? "images/dummy-img.png" 
            : FileManager.FetchFileUrl(Training.ImageUrl, Constants.FilePath.TrainingsImagesFilePath);
    }
    #endregion

    #region Inspection Details
    private GetInspectionDto Inspection { get; set; } = new();

    private async Task GetInspectionDetails(Guid inspectionId)
    {
        var inspection = await InspectionService.GetInspectionById(inspectionId);
            
        if (inspection?.Result is null)
        {
            SnackbarService.ShowSnackbar(inspection?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
            return;
        }

        Inspection = inspection.Result;
    }
    
    private bool IsInspectionModalOpen { get; set; }

    private void OpenCloseInspectionModal()
    {
        IsInspectionModalOpen = !IsInspectionModalOpen;
        
        StateHasChanged();
    }
    #endregion
}