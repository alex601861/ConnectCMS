namespace CMSTrain.Client.Models.Responses.Count;

public class ApprovalMatrixCountDto
{
    public int PendingCount { get; set; }

    public int ApprovedCount { get; set; }

    public int RejectedCount { get; set; }
}
