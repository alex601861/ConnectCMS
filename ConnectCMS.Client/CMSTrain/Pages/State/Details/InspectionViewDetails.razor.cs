using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Inspection;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.State.Details;

public partial class InspectionViewDetails
{
    [Parameter] public Guid InspectionId { get; set; }
    
    private GetInspectionDto Inspection { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await GetInspectionViewDetails();
    }

    private async Task GetInspectionViewDetails()
    {
        try
        {
            var result = await InspectionService.GetInspectionById(InspectionId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning,
                    Variant.Outlined);
                return;
            }

            Inspection = result.Result;

            if (Inspection.ImageUrl != null)
                Inspection.ImageUrl = FileManager.FetchFileUrl(Inspection.ImageUrl, Constants.FilePath.InspectionImagesFilePath);
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Filled);
        }
    }
}