using CMSTrain.Application.DTOs.Training;
using CMSTrain.Application.DTOs.Inspection;

namespace CMSTrain.Application.DTOs.Questionnaires;

public class GetTrainingQuestionnaireDto
{
    public GetTrainingDto Training { get; set; } = new();

    public GetInspectionDto Inspection { get; set; } = new();
}