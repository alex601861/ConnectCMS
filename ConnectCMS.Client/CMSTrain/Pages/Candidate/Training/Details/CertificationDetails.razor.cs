using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Certification;

namespace CMSTrain.Client.Pages.Candidate.Training.Details;

public partial class CertificationDetails : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }

    private GetCertificationDetails? Certification { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetCertificationDetails();
    }

    private async Task GetCertificationDetails()
    {
        try
        {
            var result = await CertificationService.GetCertificationDetailsByTrainingId(TrainingId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                
                return;
            }

            Certification = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
}