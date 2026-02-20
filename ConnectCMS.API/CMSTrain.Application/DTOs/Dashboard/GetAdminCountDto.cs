namespace CMSTrain.Application.DTOs.Dashboard;

public class GetAdminCountDto
{  
    public int TotalTrainings { get; set; }
    
    public double? TrainingGrowthPercent { get; set; }
    
    public int TotalRegisteredCandidates { get; set; }
    
    public double? TotalRegisteredCandidatesGrowth { get; set; }
        
    public int TotalRegisteredTrainers { get; set; }
    
    public double? TotalRegisteredTrainersGrowth { get; set; }

    public int TotalPendingRequests { get; set; }
    
    public double? TotalPendingRequestsGrowth { get; set; }
}