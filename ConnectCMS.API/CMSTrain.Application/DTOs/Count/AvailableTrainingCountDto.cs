namespace CMSTrain.Application.DTOs.Count;

public class AvailableTrainingCountDto
{
    public int AvailableCount { get; set; }

    public int PendingCount { get; set; }

    public int RejectedCount { get; set; }
}
