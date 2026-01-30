namespace CMSTrain.Application.Settings;

public class StrategicSettings
{
    public List<StrategyConfiguration> StrategicTraits { get; set; }
}

public class StrategyConfiguration
{
    public string Title { get; set; }

    public string Type { get; set; }
    
    public List<StrategyConfiguration>? Opportunities { get; set; }

    public List<StrategyConfiguration>? Threats { get; set; }
}
