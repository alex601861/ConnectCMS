using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Class;

namespace CMSTrain.Client.Pages.State.Class;

public partial class ClassDetails : ComponentBase
{
    [Parameter] public Guid ClassId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetAllClassDetails();
    }

    #region Class Details
    private GetClassDto ClassInformation { get; set; } = new();

    private async Task GetAllClassDetails()
    {
        try
        {
            var result = await ClassService.GetClassById(ClassId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            ClassInformation = result.Result;
            
            ClassInformation.ImageUrl = !string.IsNullOrEmpty(ClassInformation.ImageUrl) 
                ? FileManager.FetchFileUrl(ClassInformation.ImageUrl, Constants.FilePath.ClassesImagesFilePath) 
                : "images/dummy-img.png";
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
}