using MudBlazor;
using MudBlazor.Utilities;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using CMSTrain.Client.Models.Requests.Configuration.Training;
using CMSTrain.Client.Models.Responses.Certification;

namespace CMSTrain.Client.Pages.Administrator.Training.Details;

public partial class CertificationDetails
{
    [Parameter] public Guid TrainingId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetTrainingCertificationDetails();

        await GetTrainingCertificationTriggerDetails();
    }

    #region Certification Configuration
    private TrainingCertificationConfigurationDownload? TrainingCertificationConfiguration { get; set; }

    private async Task GetTrainingCertificationDetails()
    {
        try
        {
            var response = await ConfigurationService.GetTrainingCertificationConfigurationByKey(TrainingId, TrainingConfiguration.CERTIFICATIONS.ToString());

            TrainingCertificationConfiguration = response?.Result;

            TrainingCertificationColorConfiguration = new TrainingCertificationConfigurationDto
            {
                PrimaryColor = TrainingCertificationConfiguration?.Certification.PrimaryColor.ToMudColor() ?? new MudColor(),
                TertiaryColor = TrainingCertificationConfiguration?.Certification.TertiaryColor.ToMudColor() ?? new MudColor(),
                SecondaryColor = TrainingCertificationConfiguration?.Certification.SecondaryColor.ToMudColor() ?? new MudColor() 
            };
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Configuration Handler
    private TrainingCertificationConfigurationDto TrainingCertificationColorConfiguration { get; set; } = new();
    
    private TrainingCertificationConfigurationUpload TrainingCertificationConfigurationDetails { get; set; } = new();

    private async Task UploadTrainingCertificationDetails()
    {
        try
        {
            TrainingCertificationConfigurationDetails.Certification =
                new AbstractTrainingCertificationConfigurationUploadDto()
                {
                    PrimaryColor = TrainingCertificationColorConfiguration.PrimaryColor.ToHexCode(),
                    SecondaryColor = TrainingCertificationColorConfiguration.SecondaryColor.ToHexCode(),
                    TertiaryColor = TrainingCertificationColorConfiguration.TertiaryColor.ToHexCode(),
                    PrimaryLogo = TrainingCertificationConfigurationDetails.Certification.PrimaryLogo,
                    SecondaryLogo = TrainingCertificationConfigurationDetails.Certification.SecondaryLogo,
                    Signature = TrainingCertificationConfigurationDetails.Certification.Signature
                };
            
            var response = await ConfigurationService.SaveTrainingCertificationConfiguration(TrainingId, TrainingConfiguration.CERTIFICATIONS.ToString(), TrainingCertificationConfigurationDetails);

            TrainingCertificationConfigurationDetails.Certification.Signature = null;
            TrainingCertificationConfigurationDetails.Certification.PrimaryLogo = null;
            TrainingCertificationConfigurationDetails.Certification.SecondaryLogo = null;
            
            StateHasChanged();
            
            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (response.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(response.Message, Severity.Success, Variant.Outlined);
                    break;
                case StatusCode.Status404NotFound:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(response.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(response.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private void OnHandlePrimaryLogoUpload(IBrowserFile? file)
    {
        TrainingCertificationConfigurationDetails.Certification.PrimaryLogo = file;
    }
    
    private void OnHandleSecondaryLogoUpload(IBrowserFile? file)
    {
        TrainingCertificationConfigurationDetails.Certification.SecondaryLogo = file;
    }
    
    private void OnHandleTrainerSignatureUpload(IBrowserFile? file)
    {
        TrainingCertificationConfigurationDetails.Certification.Signature = file;
    }
    #endregion

    #region Certification Template
    private bool IsCertificationTemplateModalOpen { get; set; }

    private GetCertificationDetails CertificationTemplateDetails { get; set; } = new();
    
    private void OpenCloseCertificationTemplateModal()
    {
        CertificationTemplateDetails.TrainingId = TrainingId;
        
        IsCertificationTemplateModalOpen = !IsCertificationTemplateModalOpen;
        
        StateHasChanged();
    }
    #endregion

    #region Certification Trigger Configuration
    private TrainingCertificationTriggerConfiguration TrainingCertificationTriggerConfiguration { get; set; } = new();

    private async Task GetTrainingCertificationTriggerDetails()
    {
        try
        {
            var response = await ConfigurationService.GetTrainingCertificationTriggerConfigurationByKey(TrainingId, TrainingConfiguration.CERTIFICATION_TRIGGER.ToString());

            TrainingCertificationTriggerConfiguration = response?.Result ?? new TrainingCertificationTriggerConfiguration();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Trigger Handler
    private async Task OnCertificationTriggerValueChanged(bool value)
    {
        try
        {
            var trainingCertificationTriggerConfiguration = new TrainingCertificationTriggerConfiguration
            {
                Trigger = new AbstractTrainingCertificationTriggerConfigurationDto
                {
                    IsManual = value
                }
            };

            var response = await ConfigurationService.SaveTrainingCertificationTriggerConfiguration(TrainingId, TrainingConfiguration.CERTIFICATION_TRIGGER.ToString(), trainingCertificationTriggerConfiguration);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (response.StatusCode)
            {
                case StatusCode.Status200Ok:
                    TrainingCertificationTriggerConfiguration.Trigger.IsManual = value;
                    SnackbarService.ShowSnackbar(response.Message, Severity.Success, Variant.Outlined);
                    break;
                case StatusCode.Status404NotFound:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(response.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(response.Message, Severity.Error, Variant.Outlined);
                    break;
            }
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
}