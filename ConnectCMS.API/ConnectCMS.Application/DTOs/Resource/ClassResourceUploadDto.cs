namespace CMSTrain.Application.DTOs.Resource;

public class ClassResourceUploadDto
{
    public Guid ClassId { get; set; }
    
    public List<Guid> ResourceIds { get; set; }
}