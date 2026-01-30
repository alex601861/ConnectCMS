using CMSTrain.Application.DTOs.Inspection;

namespace CMSTrain.Application.DTOs.TrainingInspection;

public class GetTrainingInspectionDto : GetInspectionDto
{
    public Guid TrainingInspectionId { get; set; }

    public Guid? QuestionnaireId { get; set; }

    public bool IsQuestionnaireUploaded { get; set; }

    public string? UploadedDate { get; set; }
}