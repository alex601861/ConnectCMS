namespace CMSTrain.Client.Models.Responses.Resource;

public class GetResourceModuleDetailsDto : GetResourceDetailsDto
{
    public Guid ModuleId { get; set; }
    
    public string? ModuleLink { get; set; }
    
    public Guid? DetailId { get; set; }
    
    public bool IsActive { get; set; }
}