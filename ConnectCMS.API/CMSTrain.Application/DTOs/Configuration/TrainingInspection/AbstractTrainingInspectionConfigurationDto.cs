namespace CMSTrain.Application.DTOs.Configuration.TrainingInspection;

public class AbstractTrainingInspectionConfigurationDto
{
    public DateTime AccessPeriod { get; set; } = DateTime.UtcNow;
    
    public DateTime ExpirePeriod { get; set; } = DateTime.UtcNow.AddDays(1);
}