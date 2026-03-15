using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Attendance;
using CMSTrain.Client.Models.Requests.Configuration.Class;
using CMSTrain.Client.Models.Responses.Attendance;
using CMSTrain.Client.Models.Responses.Organization;

namespace CMSTrain.Client.Pages.State.Attendance;

public partial class Attendance : ComponentBase
{
    [Parameter] public Guid ClassId { get; set; }
    
    [Parameter] public bool IsEditable { get; set; }
    
    [Parameter] public EventCallback OnAttendanceCountUpdate { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await GetAllAttendanceRequest();

        await GetAllAssignedClientOrganizations();
    }
    
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
        
        await GetAllAttendanceRequest();
        
        StateHasChanged();
    }
    
    private bool? IsApproved { get; set; }

    private async Task OnAttendanceFilter()
    {
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        AttendanceRequests = null;
        
        await GetAllAttendanceRequest();
    }
    #endregion
    
    #region Get All Attendance Requests
    private CollectionDto<GetAttendanceResponseDto>? AttendanceRequests { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        AttendanceRequests = null;
        
        await GetAllAttendanceRequest();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        AttendanceRequests = null;
        
        await GetAllAttendanceRequest();
    }
    
    private async Task GetAllAttendanceRequest()
    {
        try
        {
            var result = await AttendanceService.GetAllAttendanceRequests(ClassId, PageNumber, PageSize, Search, IsApproved);

            if (result?.Result == null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            AttendanceRequests = result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Approve Reject Attendance
    private bool IsAttendanceApprovalModalOpen { get; set; }

    private AttendanceApproveRejectDto AttendanceApproveReject { get; set; } = new();
    
    private void OpenApproveRejectModal(Guid assigmentId, bool isApproved)
    {
        AttendanceApproveReject = new AttendanceApproveRejectDto
        {
            RequestId = assigmentId,
            IsApproved = isApproved
        };
        
        OpenCloseApproveRejectModal();
    }

    private async Task ApproveRejectCandidate(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseApproveRejectModal();
            return;
        }
        
        try
        {
            if (string.IsNullOrEmpty(AttendanceApproveReject.Remarks))
            {
                AttendanceApproveReject.Remarks = AttendanceApproveReject.IsApproved ? "Approved." : "Rejected.";
            }
            
            var result = await AttendanceService.ApproveRejectAttendance(AttendanceApproveReject);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    await GetAllAttendanceRequest();
                    OpenCloseApproveRejectModal();
                    await OnAttendanceCountUpdate.InvokeAsync();
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

    private void OpenCloseApproveRejectModal()
    {
        IsAttendanceApprovalModalOpen = !IsAttendanceApprovalModalOpen;
    }
    #endregion

    #region Download Attendance Upload
    private async Task DownloadAttendanceImage(Guid attendanceId)
    {
        try
        {
            var result = await AttendanceService.DownloadAttendanceImage(attendanceId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
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

    #region Attendance Configuration
    private bool IsAttendanceConfigurationModalOpen { get; set; }

    private async Task OpenCloseAttendanceConfigurationModal()
    {
        IsAttendanceConfigurationModalOpen = !IsAttendanceConfigurationModalOpen;

        await GetResourceConfiguration();
        
        StateHasChanged();
    }

    private ClassAttendanceConfiguration ClassAttendanceConfiguration { get; set; } = new();
    
    private async Task GetResourceConfiguration()
    {
        try
        {
            var result = await ConfigurationService.GetClassAttendanceConfigurationByKey(ClassId,
                ClassConfiguration.ATTENDANCE_PERIOD.ToString());
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ClassAttendanceConfiguration = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private async Task UploadAttendanceConfigurations(bool isClosed)
    {
        if (isClosed)
        {
            await OpenCloseAttendanceConfigurationModal();
            return;
        }

        try
        {
            ClassAttendanceConfiguration.Accessibility.Radius = ClassAttendanceConfiguration.Accessibility.IsLocationEnabled ? ClassAttendanceConfiguration.Accessibility.Radius : null;
            
            var result = await ConfigurationService.SaveClassAttendanceConfiguration(ClassId, ClassConfiguration.ATTENDANCE_PERIOD.ToString(), ClassAttendanceConfiguration);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await OpenCloseAttendanceConfigurationModal();
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

    #region Attendance Export

    #region Assigned Organizations
    private List<GetOrganizationDto> Organizations { get; set; } = [];

    private async Task GetAllAssignedClientOrganizations()
    {
        try
        {
            var @class = await ClassService.GetClassById(ClassId);
            
            var result = await TrainingService.GetAllAssignedClientOrganizations(@class?.Result?.TrainingId ?? Guid.Empty);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Organizations = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Export to XLSX
    private Guid OrganizationId { get; set; }
    
    private bool IsAttendanceExportModalOpen { get; set; }
    
    private void OpenCloseAttendanceExportModal()
    {
        IsAttendanceExportModalOpen = !IsAttendanceExportModalOpen;

        OrganizationId = Guid.Empty;
        
        StateHasChanged();
    }
    
    private async Task ExportAttendanceDetails(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseAttendanceExportModal();
            return;
        }

        try
        {
            var result = await AttendanceService.ExportAttendanceDetails(ClassId, OrganizationId != Guid.Empty ? OrganizationId : null);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseAttendanceExportModal();
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
    
    #endregion
}