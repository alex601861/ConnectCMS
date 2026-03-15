using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Training;

namespace CMSTrain.Client.Pages.State.Details;

public partial class TrainingViewDetails
{
    [Parameter] public Guid TrainingId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetTrainingViewDetails();
    }

    private GetTrainingDto Training { get; set; } = new();

    private async Task GetTrainingViewDetails()
    {
        try
        {
            var result = await TrainingService.GetTrainingById(TrainingId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }
            
            Training = result.Result;
            
            Training.ImageUrl = 
                !string.IsNullOrEmpty(Training.ImageUrl) 
                    ? FileManager.FetchFileUrl(Training.ImageUrl, Constants.FilePath.TrainingsImagesFilePath)
                    : "images/dummy-img.png";
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Filled);
        }
    }
}