namespace CMSTrain.Client.Models.Responses.Strategy;

// Sharable Content and Details
public class GetStrategyModuleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Type { get; set; }
}