namespace CMSTrain.Client.Models.Requests.TrainingCandidate;

public class ApproveRejectRequestDto
{
    public Guid TrainingCandidateId { get; set; }

    public bool IsApproved { get; set; }

    public string Remarks {  get; set; }    
}