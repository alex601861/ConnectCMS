using MudBlazor;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Certification;
using CMSTrain.Client.Models.Requests.Configuration.Training;

namespace CMSTrain.Client.Pages.State.Certification;

public partial class CertificationInformation
{
    [Parameter] public GetCertificationDetails Certification { get; set; } = new();
    
    protected override async Task OnInitializedAsync()
    {
        await GetTrainingCertificationDetails();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("loadHtml2Canvas");
        }
    }
    
    #region Certification Configuration
    private TrainingCertificationConfigurationDownload TrainingCertificationConfiguration { get; set; } = new();

    private async Task GetTrainingCertificationDetails()
    {
        try
        {
            if (Certification.TrainingId == Guid.Empty) return;
            
            var response = await ConfigurationService.GetTrainingCertificationConfigurationByKey(Certification.TrainingId, TrainingConfiguration.CERTIFICATIONS.ToString());

            if (response?.Result != null)
            {
                TrainingCertificationConfiguration = response.Result;
                
                var certificationFilePath = Path.Combine(Constants.FilePath.CertificationsImagesFilePath, Certification.TrainingId.ToString());

                TrainingCertificationConfiguration.Certification.PrimaryLogo =
                    !string.IsNullOrEmpty(TrainingCertificationConfiguration.Certification.PrimaryLogo)
                        ? FileManager.FetchFileUrl(TrainingCertificationConfiguration.Certification.PrimaryLogo,
                            certificationFilePath)
                        : string.Empty;
                
                TrainingCertificationConfiguration.Certification.SecondaryLogo =
                    !string.IsNullOrEmpty(TrainingCertificationConfiguration.Certification.SecondaryLogo)
                        ? FileManager.FetchFileUrl(TrainingCertificationConfiguration.Certification.SecondaryLogo,
                            certificationFilePath)
                        : string.Empty;
                
                TrainingCertificationConfiguration.Certification.Signature =
                    !string.IsNullOrEmpty(TrainingCertificationConfiguration.Certification.Signature)
                        ? FileManager.FetchFileUrl(TrainingCertificationConfiguration.Certification.Signature,
                            certificationFilePath)
                        : string.Empty;
            }
            else
            {
                TrainingCertificationConfiguration.Certification.PrimaryColor = "#18B2E6";
                TrainingCertificationConfiguration.Certification.SecondaryColor = "#232D68";
                TrainingCertificationConfiguration.Certification.TertiaryColor = "#FFFFFF";
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Download Certificate
    private bool IsDisabled { get; set; }
    
    private async Task DownloadCertificate()
    {
        IsDisabled = true;
        
        try
        {
            var certificationTitle = $"{Certification.CertificationDetails.Training} - {Certification.CertificationDetails.Candidate} Certification.pdf";
            
            await JsRuntime.InvokeAsync<string>("getCertificateHtml", "certificate-container");
            
            await JsRuntime.InvokeVoidAsync("convertToPdf", "certificate-container", certificationTitle);
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar($"Failed to download certificate due to the following reason(s): {ex.Message}", Severity.Error, Variant.Outlined);
        }
        finally
        {
            IsDisabled = false;
        }
    }
    #endregion
}