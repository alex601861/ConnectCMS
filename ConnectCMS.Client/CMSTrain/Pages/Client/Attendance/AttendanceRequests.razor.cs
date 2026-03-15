using MudBlazor;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Attendance;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Responses.Attendance;

namespace CMSTrain.Client.Pages.Client.Attendance;

public partial class AttendanceRequests : ComponentBase
{
    [Parameter] public Guid ClassId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetAllAttendanceRequest();
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
    }
    
    private bool? IsApproved { get; set; }

    private async Task OnAttendanceFilter()
    {
        await GetAllAttendanceRequest();
    }
    #endregion

    #region Attendance Details
    private CollectionDto<GetAttendanceResponseDto>? AttendanceRequestDetails { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        AttendanceRequestDetails = null;
        
        await GetAllAttendanceRequest();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        AttendanceRequestDetails = null;
        
        await GetAllAttendanceRequest();
    }
    
    private async Task GetAllAttendanceRequest()
    {
        try
        {
            var result = await AttendanceService.GetAttendanceRequestForClient(ClassId, PageNumber, PageSize, Search, IsApproved);

            if (result?.Result == null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            AttendanceRequestDetails = result;
            
            StateHasChanged();
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
}