namespace CMSTrain.Client.Models.Responses.TrainingInspection;

public class GetTrainingCandidateInspectionDto
{
    public Guid Id { get; set; }

    public Guid TrainingInspectionId { get; set; }

    public Guid TrainingCandidateId { get; set; }

    public Guid? SubordinateId { get; set; }

    public string SubordinateType { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string? Phase { get; set; }

    public InspectionResponse InspectionResponse { get; set; } = new();
}

public class InspectionResponse
{
    public Guid? QuestionnaireId { get; set; }

    public Guid? UserResponseId { get; set; }
    
    public bool IsQuestionUploaded { get; set; }

    public string? UploadedDate { get; set; }

    public bool IsQuestionnaireAnswered { get; set; }
    
    public string? AnsweredDate { get; set; }
}