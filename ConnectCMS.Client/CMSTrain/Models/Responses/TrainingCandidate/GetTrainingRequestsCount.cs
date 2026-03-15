namespace CMSTrain.Client.Models.Responses.TrainingCandidate;

public class GetTrainingRequestsCount
{
    public int TotalRequests { get; set; }

    public decimal TotalRequestsGrowthFromLastWeek { get; set; }
    
    public int TotalPendingRequests { get; set; }

    public decimal TotalPendingRequestsGrowthFromLastWeek { get; set; }
    
    public int TotalRejectedRequests { get; set; }

    public decimal TotalRejectedRequestsGrowthFromLastWeek { get; set; }
    
    public int TotalAcceptedRequests { get; set; }

    public decimal TotalAcceptedRequestsGrowthFromLastWeek { get; set; }
}