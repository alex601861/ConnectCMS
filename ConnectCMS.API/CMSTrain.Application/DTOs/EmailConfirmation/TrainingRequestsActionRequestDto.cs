namespace CMSTrain.Application.DTOs.EmailConfirmation;

public class TrainingRequestsActionRequestDto
{
    public Guid TrainingId { get; set; }

    public List<RequestAction> RequestActions { get; set; } = [];
}

public class RequestAction
{
    public Guid UserId { get; set; }
    
    public bool IsApproved { get; set; }
    
    public string Remarks { get; set; }
}