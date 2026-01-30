namespace CMSTrain.Application.DTOs.Dashboard;

public class GetQuestionnaireDto
{
    public Guid Id { get; set; }
    
    public Guid InspectionId { get; set; }
    
    public string Inspection { get; set; }
    
    public Guid TrainingId { get; set; }
    
    public string Training { get; set; }
    
    public string UploadedDate { get; set; }
}