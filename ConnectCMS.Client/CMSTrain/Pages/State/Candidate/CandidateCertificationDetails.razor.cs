using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Certification;

namespace CMSTrain.Client.Pages.State.Candidate;

public partial class CandidateCertificationDetails
{
    [Parameter] public Guid TrainingCandidateId { get; set; }
    
    private GetCertificationDetails? Certification { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetCertificationDetails();
    }

    #region Certification Details
    private async Task GetCertificationDetails()
    {
        try
        {
            var result = await CertificationService.GetCertificationDetailsByTrainingCandidateId(TrainingCandidateId);

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
    #endregion

    #region Certification Modal Details
    private bool IsCertificationDetailsModalOpen { get; set; }

    private void OpenCloseCertificationDetailsModal()
    {
        IsCertificationDetailsModalOpen = !IsCertificationDetailsModalOpen;
    }
    #endregion
}