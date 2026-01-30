namespace CMSTrain.Application.DTOs.Dashboard;

public class GetUpcomingTrainingsDto : GetPopularTrainingDto
{
    public int PendingRequests { get; set; }
}