namespace CMSTrain.Application.DTOs.Configuration.Class;

public class AbstractClassAttendanceConfigurationDto
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    public TimeSpan AccessPeriod { get; set; }
    
    public TimeSpan ExpirePeriod { get; set; }
    
    public bool IsLocationEnabled { get; set; }
    
    public double? Radius { get; set; }
}