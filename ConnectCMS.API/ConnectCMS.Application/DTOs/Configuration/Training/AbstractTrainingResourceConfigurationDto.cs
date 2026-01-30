namespace CMSTrain.Application.DTOs.Configuration.Training;

public class AbstractTrainingResourceConfigurationDto
{
    public DateTime AccessPeriod { get; set; } = DateTime.UtcNow;
    
    public DateTime ExpirePeriod { get; set; } = DateTime.UtcNow.AddDays(1);
}