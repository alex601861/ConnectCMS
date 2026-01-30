using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Application.DTOs.Strategy;

public class InsertStrategyDto
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public StrategicType Type { get; set; }
}