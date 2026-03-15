using CMSTrain.Client.Models.Responses.Inspection;

namespace CMSTrain.Client.Models.Responses.TrainingInspection;

public class GetTrainingInspectionDto : GetInspectionDto
{
    public Guid TrainingInspectionId { get; set; }

    public Guid? QuestionnaireId { get; set; }

    public bool IsQuestionnaireUploaded { get; set; }

    public string? UploadedDate { get; set; }
}