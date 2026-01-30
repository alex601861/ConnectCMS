namespace CMSTrain.Application.DTOs.Resource;

public class TrainingResourceUploadDto
{
    public Guid TrainingId { get; set; }
    
    public List<Guid> ResourceIds { get; set; }
}