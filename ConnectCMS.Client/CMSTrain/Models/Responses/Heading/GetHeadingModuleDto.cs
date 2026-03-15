namespace CMSTrain.Client.Models.Responses.Heading;

public class GetHeadingModuleDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string Type { get; set; }
    
    public Guid? ParentHeadingId { get; set; }
    
    public bool IsActive { get; set; }
}