using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Responses.Count;
using CMSTrain.Client.Models.Responses.Class;
using CMSTrain.Client.Models.Responses.Attendance;

namespace CMSTrain.Client.Pages.Candidate.Class;

public partial class Class : ComponentBase
{
    [Parameter] public Guid ClassId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAllClassDetails();
        await GetClassDetailsCount();
        await GetAttendanceRequestForCandidate();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Class;
    }
    #endregion

    #region Panel Navigation
    private int ActivePanelIndex { get; set; }
    #endregion
    
    #region Class Details
    private GetClassDto ClassDetails { get; set; } = new();

    private async Task GetAllClassDetails()
    {
        try
        {
            var result = await ClassService.GetClassById(ClassId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ClassDetails = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    #endregion

    #region Class Details Count for Candidates
    private ClassCountDto ClassCountDto { get; set; } = new();

    private async Task GetClassDetailsCount()
    {
        try
        {
            var result = await ClassService.GetClassCountForCandidate(ClassId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ClassCountDto = result.Result;
        }
        catch(Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Attendance Details
    public GetAttendanceResponseDto? Attendance { get; set; }
    
    private async Task GetAttendanceRequestForCandidate()
    {
        try
        {
            var response = await AttendanceService.GetAttendanceRequestForCandidate(ClassId);
            
            if (response?.Result is null)
            {
                Attendance = new GetAttendanceResponseDto();
                return;
            }

            Attendance = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private async Task HandleAttendanceUpload()
    {
        await GetAttendanceRequestForCandidate();

        await GetClassDetailsCount();
        
        StateHasChanged();
    }
    #endregion
}