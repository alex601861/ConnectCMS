namespace CMSTrain.Client.Models.Responses.Dashboard;

public class GetUpcomingTrainingDto : GetPopularTrainingDto
{
    public int PendingRequests { get; set; }
}