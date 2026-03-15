namespace CMSTrain.Client.Models.Requests.Configuration.TrainingInspection;

public class AbstractTrainingInspectionConfigurationDto
{
    public DateTime? AccessPeriod { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpirePeriod { get; set; } = DateTime.UtcNow.AddDays(7);
}