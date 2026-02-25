using Blazorise.Extensions;
using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Configuration.TrainingInspection;
using CMSTrain.Client.Models.Requests.Questionnaires;
using CMSTrain.Client.Models.Responses.Inspection;
using CMSTrain.Client.Models.Requests.TrainingInspection;
using CMSTrain.Client.Models.Responses.TrainingInspection;
using TrainingInspectionConfiguration = CMSTrain.Client.Models.Requests.Configuration.TrainingInspection.TrainingInspectionConfiguration;

namespace CMSTrain.Client.Pages.Administrator.Training.Details;

public partial class QuestionnaireDetails : ComponentBase
{
    [Parameter] public Guid TrainingId { get; set; }
    
    [Parameter] public EventCallback OnInspectionDetailsCountUpdate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetAllAssignedInspections();
        
        await GetAllAssignedTrainingInspections();

        await GetAllAvailableTrainingInspections();

        await GetTrainingInspectionQuestionnairesCount();
    }

    #region Available Inspections
    private List<GetInspectionDto> AvailableInspections { get; set; } = [];

    private async Task GetAllAvailableTrainingInspections()
    {
        try
        {
            var response = await InspectionService.GetAllAvailableTrainingInspections();

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            AvailableInspections = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Search and Filter
    private string _search = string.Empty;
    
    private string Search
    {
        get => _search;
        set
        {
            if (_search == value) return;
            _search = value;
            _ = OnSearchInputAsync(_search);
        }
    }
    
    private async Task OnSearchInputAsync(string search)
    {
        Search = search;
        
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        await GetAllAssignedTrainingInspections();
    }
    #endregion
    
    #region Training Inspection for Assigned Details
    private CollectionDto<GetTrainingInspectionDto>? AssignedTrainingInspections { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        AssignedTrainingInspections = null;
        
        await GetAllAssignedTrainingInspections();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        AssignedTrainingInspections = null;
        
        await GetAllAssignedTrainingInspections();
    }
    
    private async Task GetAllAssignedTrainingInspections()
    {
        try
        {
            var response = await TrainingInspectionService.GetAllAssignedTrainingInspections(TrainingId, PageNumber, PageSize, Search);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            AssignedTrainingInspections = response;

            foreach (var inspection in AssignedTrainingInspections.Result)
            {
                inspection.ImageUrl = !string.IsNullOrEmpty(inspection.ImageUrl) 
                    ? FileManager.FetchFileUrl(inspection.ImageUrl, Constants.FilePath.InspectionImagesFilePath)
                    : "images/dummy-img.png";
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    } 
    #endregion

    #region Assigned Inspections
    private List<GetInspectionDto> AssignedInspections { get; set; } = [];

    private async Task GetAllAssignedInspections()
    {
        try
        {
            var response = await InspectionService.GetAllAssignedTrainingInspections(TrainingId);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            AssignedInspections = response.Result;

            foreach (var inspection in AssignedInspections.Where(inspection => !string.IsNullOrEmpty(inspection.ImageUrl)))
            {
                if (inspection.ImageUrl != null)
                    inspection.ImageUrl = FileManager.FetchFileUrl(inspection.ImageUrl, Constants.FilePath.InspectionImagesFilePath);
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Assign Training Inspections
    private bool IsTrainingInspectionModalOpen { get; set; }

    private IReadOnlyCollection<Guid> AssignedInspectionIds { get; set; } = [];
    
    private bool IsQuestionnaireNotAssignedDisabled =>
        AssignedInspectionIds.IsNullOrEmpty();

    private void OpenCloseTrainingInspectionModal()
    {
        IsTrainingInspectionModalOpen = !IsTrainingInspectionModalOpen;
        
        StateHasChanged();
    }
    
    private AssignTrainingInspectionDto AssignTrainingInspection { get; set; } = new();

    private void OpenTrainingInspectionModal()
    {
        AssignedInspectionIds = AssignedInspections.Select(x => x.Id).ToList();

        AssignTrainingInspection = new AssignTrainingInspectionDto()
        {
            TrainingId = TrainingId,
            InspectionId = AssignedInspectionIds.ToList()
        };
            
        OpenCloseTrainingInspectionModal();
    }

    private async Task OnAssignTrainingInspections(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseTrainingInspectionModal();
            
            return;
        }

        try
        {
            AssignTrainingInspection.InspectionId = AssignedInspectionIds.ToList();
            
            var result = await TrainingInspectionService.AssignTrainingInspections(AssignTrainingInspection);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseTrainingInspectionModal();
                    await GetAllAssignedInspections();
                    await GetAllAssignedTrainingInspections();
                    await OnInspectionDetailsCountUpdate.InvokeAsync();
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
    
    #region Questionnaires Count
    private GetTrainingInspectionQuestionnaireCountDto TrainingInspectionQuestionnaireSummary { get; set; } = new();
    
    private async Task GetTrainingInspectionQuestionnairesCount()
    {
        try
        {
            var result = await TrainingInspectionService.GetTrainingInspectionQuestionnairesCount(TrainingId);
    
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
    
            TrainingInspectionQuestionnaireSummary = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Training Inspection Configurations
    private Guid TrainingInspectionId { get; set; } = Guid.Empty;
    
    private bool IsTrainingInspectionConfigurationModalOpen { get; set; }

    private TrainingInspectionConfiguration TrainingInspectionConfigurationModel { get; set; } = new();

    private void AddOrRemoveTrainingInspectionConfigurationAccessibility(bool isAdded, AbstractTrainingInspectionConfigurationDto accessibility)
    {
        if (isAdded)
        {
            var trainingInspectionConfiguration = TrainingInspectionConfigurationModel.Accessibility.LastOrDefault();
            
            if (trainingInspectionConfiguration is not null)
            {
                var accessPeriod = trainingInspectionConfiguration.ExpirePeriod?.AddDays(1) ?? DateTime.Now;
                
                var expirePeriod = accessPeriod.AddDays(7);
                
                TrainingInspectionConfigurationModel.Accessibility.Add(new AbstractTrainingInspectionConfigurationDto()
                {
                    AccessPeriod = accessPeriod,
                    ExpirePeriod = expirePeriod
                });
            }
            else
            {
                var accessPeriod = DateTime.Now;
                
                var expirePeriod = accessPeriod.AddDays(7);
                
                TrainingInspectionConfigurationModel.Accessibility.Add(new AbstractTrainingInspectionConfigurationDto()
                {
                    AccessPeriod = accessPeriod,
                    ExpirePeriod = expirePeriod
                });
            }   
        }
        else
        {
            TrainingInspectionConfigurationModel.Accessibility.Remove(accessibility);
        }
    }
    
    private void OpenCloseTrainingInspectionConfigurationModal(Guid trainingInspectionId)
    {
        IsTrainingInspectionConfigurationModalOpen = !IsTrainingInspectionConfigurationModalOpen;

        TrainingInspectionId = IsTrainingInspectionConfigurationModalOpen ? trainingInspectionId : Guid.Empty;
        
        StateHasChanged();
    }
    
    private async Task OnTrainingInspectionConfiguration(Guid trainingInspectionId)
    {
        try
        {
            var result = await ConfigurationService.GetTrainingInspectionConfigurationByKey(trainingInspectionId, CMSTrain.Client.Models.Constants.TrainingInspectionConfiguration.RESPONSE_PERIOD.ToString());

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            TrainingInspectionConfigurationModel = result.Result;
            
            OpenCloseTrainingInspectionConfigurationModal(trainingInspectionId);
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private async Task SaveTrainingInspectionConfiguration(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseTrainingInspectionConfigurationModal(Guid.Empty);
            
            return;
        }

        try
        {
            foreach (var accessibility in TrainingInspectionConfigurationModel.Accessibility)
            {
                if (accessibility.ExpirePeriod != null)
                {
                    accessibility.ExpirePeriod = accessibility.ExpirePeriod.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }
            
            var result = await ConfigurationService.SaveTrainingInspectionConfiguration(TrainingInspectionId, CMSTrain.Client.Models.Constants.TrainingInspectionConfiguration.RESPONSE_PERIOD.ToString(), TrainingInspectionConfigurationModel);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllAssignedTrainingInspections();
                    OpenCloseTrainingInspectionConfigurationModal(Guid.Empty);
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

    private void ChangeExpireDatePeriod(DateTime? accessPeriod, AbstractTrainingInspectionConfigurationDto trainingInspectionConfiguration)
    {
        trainingInspectionConfiguration.AccessPeriod = accessPeriod;
        
        if (trainingInspectionConfiguration.ExpirePeriod < trainingInspectionConfiguration.AccessPeriod)
        {
            trainingInspectionConfiguration.ExpirePeriod = trainingInspectionConfiguration.AccessPeriod?.AddDays(7);
        }    
    }
    
    private DateTime? ValidateAccessPeriod(AbstractTrainingInspectionConfigurationDto trainingInspectionConfiguration)
    {
        var index = TrainingInspectionConfigurationModel.Accessibility.IndexOf(trainingInspectionConfiguration);

        if (index <= 0) return null;
        
        var previousConfiguration = TrainingInspectionConfigurationModel.Accessibility[index - 1];

        return previousConfiguration.ExpirePeriod;

    }
    #endregion
    
    #region Navigation
    private void NavigateToQuestionDetailsForm(Guid questionnaireId)
    {
        NavigationManager.NavigateTo($"/admin/questionnaire-view-form/{questionnaireId}");
    }
    
    private void NavigateToQuestionUploadForm(Guid trainingInspectionId)
    {
        NavigationManager.NavigateTo($"/admin/questionnaire-upload-form/{trainingInspectionId}");
    }
    #endregion

    #region QR Code
    private bool IsQuestionnaireQrCodeModalOpen { get; set; }
    
    private string QrCodeImage { get; set; } = string.Empty;

    private QuestionnaireQrCodeDto QuestionnaireQrCode { get; set; } = new();
    
    private async Task OpenCloseGenerateQuestionnaireQrModal(Guid questionnaireId, string type)
    {
        IsQuestionnaireQrCodeModalOpen = !IsQuestionnaireQrCodeModalOpen;

        if (IsQuestionnaireQrCodeModalOpen)
        {
            QuestionnaireQrCode = new QuestionnaireQrCodeDto()
            {
                QuestionnaireId = questionnaireId,
                InspectionType = type
            };

            await GenerateQuestionnaireQrCode(false);
        }
        else
        {
            QrCodeImage = string.Empty;
            
            QuestionnaireQrCode = new QuestionnaireQrCodeDto();
        }
        
        StateHasChanged();
    }
    
    private async Task GenerateQuestionnaireQrCode(bool isClosed)
    {
        if (isClosed)
        {
            IsQuestionnaireQrCodeModalOpen = false;
            StateHasChanged();
            return;
        }

        try
        {
            var result = await QuestionnaireService.GenerateQuestionnaireAnswerUploadFormQrCode(QuestionnaireQrCode.QuestionnaireId, QuestionnaireQrCode.InspectionType);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    if (result.Result.Length == 0)
                    {
                        QrCodeImage = string.Empty;
                        SnackbarService.ShowSnackbar("QR Code of the respective questionnaire could not be downloaded.", Severity.Warning, Variant.Outlined);
                    }
                    else
                    {
                        var base64Image = Convert.ToBase64String(result.Result);
                        QrCodeImage = $"data:image/png;base64,{base64Image}";
                    }
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
    
    private async Task DownloadQuestionnaireQrCode(bool isClosed)
    {
        if (isClosed)
        {
            await GenerateQuestionnaireQrCode(true);
            
            return;
        }
        
        try
        {
            var questionnaireModule = new ResourceDownloadQrCodeDto()
            {
                QuestionnaireId = QuestionnaireQrCode.QuestionnaireId,
                InspectionType = QuestionnaireQrCode.InspectionType
            };
            
            var result = await QuestionnaireService.DownloadQuestionnaireAnswerUploadFormQrCode(questionnaireModule);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    await GenerateQuestionnaireQrCode(true);
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
    
    #region Export To Excel
    private int Phase { get; set; }
    
    private bool IsExportQuestionnaireModalOpen { get; set; }

    private GetTrainingInspectionDetailsDto TrainingInspection { get; set; } = new();

    private async Task OpenCloseExportQuestionnaireModal(Guid questionnaireId)
    {
        try
        {
            Phase = 1;
            
            IsExportQuestionnaireModalOpen = !IsExportQuestionnaireModalOpen;

            if (IsExportQuestionnaireModalOpen)
            {
                await GetTrainingAndInspectionDetails(questionnaireId);
            }
            else
            {
                TrainingInspection = new GetTrainingInspectionDetailsDto();
            }
        
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private async Task GetTrainingAndInspectionDetails(Guid questionnaireId)
    {
        try
        {
            var trainingInspection = await TrainingInspectionService.GetTrainingInspectionByQuestionnaire(questionnaireId);

            if (trainingInspection?.Result is null)
            {
                SnackbarService.ShowSnackbar(trainingInspection?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return;
            }

            TrainingInspection = trainingInspection.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private async Task ExportQuestionnaireDetails(bool isClosed)
    {
        if (isClosed)
        {
            await OpenCloseExportQuestionnaireModal(TrainingInspection.QuestionnaireId);
            
            return;
        }
        
        try
        {
            var result = await QuestionnaireService.ExportQuestionnaireDetails(TrainingInspection.QuestionnaireId, Phase);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await OpenCloseExportQuestionnaireModal(TrainingInspection.QuestionnaireId);
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