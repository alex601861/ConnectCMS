namespace CMSTrain.Application.DTOs.Strategy;

public class UploadStrategyDetailsDto
{
    public Guid StrategyId { get; set; }
    
    public List<Guid> Opportunities { get; set; }

    public List<Guid> Threats { get; set; }
}