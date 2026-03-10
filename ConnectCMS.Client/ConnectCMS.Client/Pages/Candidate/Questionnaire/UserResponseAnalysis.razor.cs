using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Analysis;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Models.Responses.Analysis;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.Candidate.Questionnaire;

public partial class UserResponseAnalysis
{
    [Parameter] public Guid UserResponseId { get; set; }
    
    private GetUserResponseAnalysisDto UserResponseAnalyses { get; set; } = new();
    
    protected override async Task OnInitializedAsync()
    {
        await GetUserRole();
        
        await GetUserResponseAnalysisDetails();
    }

    #region Role
    private RolesDto Role { get; set; } = new();

    private async Task GetUserRole()
    {
        try
        {
            var response = await ProfileService.GetUserRole();

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Role = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region User Analysis
    private async Task GetUserResponseAnalysisDetails()
    {
        var response = await AnalysisService.GetUserResponseAnalysisDetailsForFeedbacks(UserResponseId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        UserResponseAnalyses = response.Result;
        
        StateHasChanged();
    }
    #endregion

    #region Upload User Analysis
    private UploadUserResponseAnalysisDto ResponseAnalysis { get; set; } = new();

    private async Task UploadUserResponseAnalysis()
    {
        try
        {
            ResponseAnalysis.UserResponseId = UserResponseId;

            var response = await AnalysisService.UploadUserResponseAnalysis(ResponseAnalysis);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (response.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetUserResponseAnalysisDetails();
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
    #endregion
}