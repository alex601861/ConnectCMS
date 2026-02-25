using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Extensions;
using CMSTrain.Client.Models.Responses.Heading;
using CMSTrain.Client.Models.Responses.Inspection;
using CMSTrain.Client.Models.Responses.Questionnaires;
using CMSTrain.Client.Models.Responses.TrainingInspection;

namespace CMSTrain.Client.Pages.Administrator.Questionnaire;

public partial class QuestionnaireDetails
{
    [Parameter] public Guid QuestionnaireId { get; set; }

    private FacetType FacetType { get; set; } = FacetType.Facet;
    
    private InspectionType InspectionType { get; set; } = InspectionType.PersonalityTest;
    
    protected override async Task OnInitializedAsync()
    {
        var questionnaire = await GetQuestionnaireDetails();

        var trainingInspection = await GetTrainingAndInspectionDetails(questionnaire.TrainingInspectionId ?? Guid.Empty);

        var inspectionDetail = await GetInspectionDetails(trainingInspection.InspectionId);

        MapInspectionCategories(inspectionDetail);
        
        await GetAllHeadings(inspectionDetail);

        await GetAllSubHeadings();
        
        StateHasChanged();
    }

    #region Training Inspection
    private GetTrainingInspectionDetailsDto TrainingInspection { get; set; } = new();

    private async Task<GetTrainingInspectionDetailsDto> GetTrainingAndInspectionDetails(Guid trainingInspectionId)
    {
        try
        {
            if (trainingInspectionId == Guid.Empty) return new GetTrainingInspectionDetailsDto();

            var trainingInspection = await TrainingInspectionService.GetTrainingInspectionById(Questionnaire.TrainingInspectionId ?? Guid.Empty);

            if (trainingInspection?.Result is null)
            {
                SnackbarService.ShowSnackbar(trainingInspection?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return new GetTrainingInspectionDetailsDto();
            }

            TrainingInspection = trainingInspection.Result;

            return TrainingInspection;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        
        return new GetTrainingInspectionDetailsDto();
    }
    #endregion
    
    #region Inspection Details
    private GetInspectionDto Inspection { get; set; } = new();

    private async Task<GetInspectionDto> GetInspectionDetails(Guid inspectionId)
    {
        if (inspectionId == Guid.Empty) return new GetInspectionDto();

        var inspection = await InspectionService.GetInspectionById(inspectionId);
            
        if (inspection?.Result is null)
        {
            SnackbarService.ShowSnackbar(inspection?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
            return new GetInspectionDto();
        }

        Inspection = inspection.Result;

        return Inspection;
    }
    #endregion
    
    #region Questionnaire and Question Details
    private GetQuestionnaireDto Questionnaire { get; set; } = new();

    private async Task<GetQuestionnaireDto> GetQuestionnaireDetails()
    {
        try
        {
            if (QuestionnaireId == Guid.Empty) return new GetQuestionnaireDto();
            
            var result = await QuestionnaireService.GetQuestionnaireDetails(QuestionnaireId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Warning, Variant.Outlined);
                
                return new GetQuestionnaireDto();
            }

            Questionnaire = result.Result;

            return Questionnaire;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        
        return new GetQuestionnaireDto();
    }
    #endregion

    #region Headings and Sub-Headings
    private List<GetHeadingModuleDto> Headings { get; set; } = [];

    private List<GetHeadingModuleDto> SubHeadings { get; set; } = [];

    private void MapInspectionCategories(GetInspectionDto inspection)
    {
        if (inspection.Type.ToInspectionTypeString() == InspectionType.Feedback)
        {
            FacetType = FacetType.Division;
            InspectionType = InspectionType.Feedback;
        }
        else if (inspection.Type.ToInspectionTypeString() == InspectionType.PersonalAssessment)
        {
            FacetType = FacetType.Heading;
            InspectionType = InspectionType.PersonalAssessment;
        }
        else if (inspection.Type.ToInspectionTypeString() == InspectionType.PersonalityTest)
        {
            FacetType = FacetType.Facet;
            InspectionType = InspectionType.PersonalityTest;
        }
    }
    
    private async Task GetAllHeadings(GetInspectionDto inspection)
    {
        try
        {
            if (inspection.Id == Guid.Empty) return;
            
            var result = await HeadingService.GetAllParentHeadings(FacetType, InspectionType);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            Headings = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private async Task GetAllSubHeadings()
    {
        try
        {
            var result = await HeadingService.GetAllSubHeadings();

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            SubHeadings = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private List<GetHeadingModuleDto> GetSubHeadingsByParentId(Guid parentId)
    {
        return SubHeadings.Where(x => x.ParentHeadingId == parentId).ToList();
    }
    #endregion
}