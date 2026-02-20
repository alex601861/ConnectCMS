namespace CMSTrain.Application.DTOs.Dashboard;

public class GetDashboardCount
{
    public int TotalAssignedClass { get; set; }
    
    public double? TotalAssignedClassGrowth { get; set; }
    
    public int TotalGradedInspections { get; set; }
    
    public double? TotalGradedInspectionsGrowth { get; set; }
    
    public int PendingAttendances { get; set; } 
    
    public double? PendingAttendancesGrowth { get; set; }
}