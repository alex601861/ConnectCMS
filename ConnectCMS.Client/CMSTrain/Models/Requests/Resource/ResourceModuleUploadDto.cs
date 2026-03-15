namespace CMSTrain.Client.Models.Requests.Resource;

public class ResourceModuleUploadDto
{
    public Guid ModuleId { get; set; }
    
    public bool IsMaterialForTraining { get; set; }

    public ResourceUploadDto Resource { get; set; } = new();
}