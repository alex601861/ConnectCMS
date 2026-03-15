namespace CMSTrain.Client.Models.Requests.Questionnaires;

public class ResourceDownloadQrCodeDto
{
    public Guid QuestionnaireId { get; set; }
    
    public string InspectionType { get; set; }
}