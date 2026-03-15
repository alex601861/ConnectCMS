using CMSTrain.Client.Models.Responses.Inspection;
using CMSTrain.Client.Models.Responses.Training;

namespace CMSTrain.Client.Models.Responses.Answers;

public class GetTrainingQuestionnaireDto
{
    public GetTrainingDto Training { get; set; } = new();

    public GetInspectionDto Inspection { get; set; } = new();
}