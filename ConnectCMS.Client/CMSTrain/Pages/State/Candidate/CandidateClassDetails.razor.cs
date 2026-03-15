using CMSTrain.Client.Models.Base;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Class;

namespace CMSTrain.Client.Pages.State.Candidate;

public partial class CandidateClassDetails : ComponentBase
{
    [Parameter] public Guid TrainingCandidateId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetClassDetails();
    }

    #region Class Details
    private CollectionDto<GetClassForCandidatesDto>? ClassDetails { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        ClassDetails = null;
        
        await GetClassDetails();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        ClassDetails = null;
        
        await GetClassDetails();
    }
    
    private async Task GetClassDetails()
    {
        try
        {
            var result = await ClassService.GetAllCandidateClasses(TrainingCandidateId, PageNumber, PageSize);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                return;
            }

            ClassDetails = result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
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