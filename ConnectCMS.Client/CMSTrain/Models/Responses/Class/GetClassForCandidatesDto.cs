namespace CMSTrain.Client.Models.Responses.Class;

public class GetClassForCandidatesDto : GetClassDto
{
    public Guid? AttendanceId { get; set; }
    
    public string AttendanceMarkedStatus { get; set; }
    
    public string AttendanceApprovedStatus { get; set; }
}