using CMSTrain.Client.Models.Constants;

namespace CMSTrain.Client.Models.Requests.Strategy;

public class InsertStrategyDto
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public StrategicType Type { get; set; } = StrategicType.None;
}