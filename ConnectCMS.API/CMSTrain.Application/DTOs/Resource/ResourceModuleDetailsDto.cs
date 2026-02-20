namespace CMSTrain.Application.DTOs.Resource;

public class ResourceModuleDetailsDto : ResourceDetailsDto
{
    public Guid ModuleId { get; set; }

    public string? ModuleLink { get; set; }
    
    public Guid DetailId { get; set; }
    
    public string AssignedDate { get; set; }
    
    public bool IsActive { get; set; }
}