using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Attendance;

namespace CMSTrain.Client.Pages.Candidate.Attendance;

public partial class AttendanceDetails : ComponentBase
{
    [Parameter] public Guid ClassId { get; set; }

    [Parameter] public GetAttendanceResponseDto Attendance { get; set; } = new();
    
    [Parameter] public EventCallback OnAttendanceMarkup { get; set; }

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
    
    #region Cancel Attendance
    private bool IsAttendanceCancelModalOpen { get; set; }

    private void OpenCloseAttendanceCancelModal()
    {
        IsAttendanceCancelModalOpen = !IsAttendanceCancelModalOpen;
        
        StateHasChanged();
    }

    private void OpenAttendanceCancelModal()
    {
        OpenCloseAttendanceCancelModal();
    }

    private async Task OnCancelAttendance(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseAttendanceCancelModal();
            return;
        }

        try
        {
            var result = await AttendanceService.CancelAttendance(ClassId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseAttendanceCancelModal();
                    await OnAttendanceMarkup.InvokeAsync();
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