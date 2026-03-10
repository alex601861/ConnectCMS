using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Analysis;
using CMSTrain.Client.Models.Responses.Answers;
using CMSTrain.Client.Models.Responses.Candidate;
using CMSTrain.Client.Models.Responses.Inspection;
using CMSTrain.Client.Models.Responses.Subordinate;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Responses.TrainingInspection;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Candidate.Questionnaire;

public partial class UserResponseDetails
{
    [Parameter] public Guid UserResponseId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var answerDetails = await GetAnswerDetails();

        var trainingInspection = await GetTrainingAndInspectionDetails(answerDetails.TrainingInspectionId);

        await GetTrainingDetails(trainingInspection.TrainingId);

        await GetInspectionDetails(trainingInspection.InspectionId);

        await GetCandidateDetails();

        if (answerDetails.IsAnsweredBySubordinate) await GetSubordinateDetails();
    }

    #region Answer Details
    private GetAnswerDetailsDto AnswerDetails { get; set; } = new();

    private async Task<GetAnswerDetailsDto> GetAnswerDetails()
    {
        try
        {
            var answerDetails = await AnswerService.GetQuestionAnswerDetails(UserResponseId);

            if (answerDetails?.Result is null)
            {
                SnackbarService.ShowSnackbar(answerDetails?.Message ?? Constants.Message.ExceptionMessage,
                    Severity.Warning, Variant.Outlined);

                return new GetAnswerDetailsDto();
            }

            AnswerDetails = answerDetails.Result;

            return AnswerDetails;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }

        return new GetAnswerDetailsDto();
    }
    #endregion

    #region Training Inspection Details

    private GetTrainingInspectionDetailsDto TrainingInspection { get; set; } = new();

    private async Task<GetTrainingInspectionDetailsDto> GetTrainingAndInspectionDetails(Guid trainingInspectionId)
    {
        try
        {
            var trainingInspection = await TrainingInspectionService.GetTrainingInspectionById(trainingInspectionId);

            if (trainingInspection?.Result is null)
            {
                SnackbarService.ShowSnackbar(trainingInspection?.Message ?? Constants.Message.ExceptionMessage,
                    Severity.Error, Variant.Outlined);

                return new GetTrainingInspectionDetailsDto();
            }

            TrainingInspection = trainingInspection.Result;

            return TrainingInspection;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }

        return new GetTrainingInspectionDetailsDto();
    }

    #endregion

    #region Training Details

    private GetTrainingDto Training { get; set; } = new();

    private async Task GetTrainingDetails(Guid trainingId)
    {
        try
        {
            var training = await TrainingService.GetTrainingById(trainingId);

            if (training?.Result is null)
            {
                SnackbarService.ShowSnackbar(training?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);

                return;
            }

            Training = training.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    #endregion

    #region Inspection Details
    private GetInspectionDto Inspection { get; set; } = new();

    private async Task GetInspectionDetails(Guid inspectionId)
    {
        try
        {
            var inspection = await InspectionService.GetInspectionById(inspectionId);

            if (inspection?.Result is null)
            {
                SnackbarService.ShowSnackbar(inspection?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);

                return;
            }

            Inspection = inspection.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private bool IsInspectionModalOpen { get; set; }

    private void OpenCloseInspectionModal()
    {
        IsInspectionModalOpen = !IsInspectionModalOpen;
        
        StateHasChanged();
    }
    #endregion

    #region Candidate Details

    private GetCandidateDetailsDto Candidate { get; set; } = new();

    private async Task GetCandidateDetails()
    {
        try
        {
            var result = await CandidateService.GetCandidateDetailsById(AnswerDetails.CandidateId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);

                return;
            }

            Candidate = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    #endregion

    #region Subordinate Details (Optional)

    private GetSubordinateDto Subordinate { get; set; } = new();

    private async Task GetSubordinateDetails()
    {
        try
        {
            if (AnswerDetails.SubordinateId == null || AnswerDetails.SubordinateId == Guid.Empty)
            {
                SnackbarService.ShowSnackbar(Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);

                return;
            }

            var result = await SubordinateService.GetSubordinateById(AnswerDetails.SubordinateId ?? Guid.Empty);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);

                return;
            }

            Subordinate = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    #endregion

    #region Analysis

    private bool IsAnalysisUploadModalOpen { get; set; }
    private bool IsAnalysisDetailsModalOpen { get; set; }
    private UploadUserResponseAnalysisDto Analysis { get; set; } = new();

    private void OpenCloseAnalysisUploadModal()
    {
        IsAnalysisUploadModalOpen = !IsAnalysisUploadModalOpen;

        StateHasChanged();
    }

    private void OpenCloseAnalysisDetailsModal()
    {
        IsAnalysisDetailsModalOpen = !IsAnalysisDetailsModalOpen;

        StateHasChanged();
    }

    private void OpenAnalysisUploadModal()
    {
        Analysis = new UploadUserResponseAnalysisDto()
        {
            UserResponseId = AnswerDetails.Id
        };

        OpenCloseAnalysisUploadModal();
    }

    private async Task OnAnalysisUpload(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseAnalysisUploadModal();

            return;
        }

        try
        {
            var result = await AnalysisService.UploadUserResponseAnalysis(Analysis);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAnswerDetails();
                    OpenCloseAnalysisUploadModal();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
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
    }

    #endregion
}