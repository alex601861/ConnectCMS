namespace CMSTrain.Application.DTOs.Configuration.Class;

public class AbstractClassResourceConfigurationDto
{
    public DateTime AccessPeriod { get; set; } = DateTime.UtcNow;
    
    public DateTime ExpirePeriod { get; set; } = DateTime.UtcNow.AddDays(1);
}