using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Training;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Administrator.Training.Details;

public partial class TrainingDetails : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }

    private string ImageUrl { get; set; } = "";
    
    private GetTrainingDto TrainingDetail { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await GetTrainingDetails();
    }

    #region Training Details

    private async Task GetTrainingDetails()
    {
        try
        {
            var response = await TrainingService.GetTrainingById(TrainingId);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            TrainingDetail = response.Result;

            TrainingDetail.ImageUrl = !string.IsNullOrEmpty(TrainingDetail.ImageUrl)
                ? FileManager.FetchFileUrl(TrainingDetail.ImageUrl, Constants.FilePath.TrainingsImagesFilePath)
                : "images/dummy-img.png";

            GetStyle();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private string GetStyle()
    {
        return $"background-image: url('{ImageUrl}'); background-size: cover; background-position: center;";
    }

    #endregion
}