namespace CMSTrain.Client.Models.Requests.Resource;

public class ClassResourceUploadDto
{
    public Guid ClassId { get; set; }
    
    public List<Guid> ResourceIds { get; set; } = [];
}