namespace CMSTrain.Application.DTOs.Questionnaires;

public class QuestionnaireDownloadQrCodeDto
{
    public Guid QuestionnaireId { get; set; }
    
    public string InspectionType { get; set; }
}