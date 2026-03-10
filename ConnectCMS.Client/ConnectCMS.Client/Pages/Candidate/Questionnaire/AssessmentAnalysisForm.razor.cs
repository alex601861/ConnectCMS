using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.Analysis;

namespace CMSTrain.Client.Pages.Candidate.Questionnaire;

public partial class AssessmentAnalysisForm
{
    [Parameter] public Guid UserResponseId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetAssessmentResponseAnalysis();
    }

    private List<GetAssessmentResponseAnalysisDto> AssessmentResponseAnalysis { get; set; } = new();

    private async Task GetAssessmentResponseAnalysis()
    {
        try
        {
            var result = await AnalysisService.GetUserResponseAnalysisDetailsForAssessments(UserResponseId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                
                return;
            }
            
            AssessmentResponseAnalysis = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Warning, Variant.Outlined);
        }
    }
}