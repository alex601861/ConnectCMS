namespace CMSTrain.Application.DTOs.Class;

public class GetClassForCandidatesDto : GetClassDto
{
    public Guid? AttendanceId { get; set; }
    
    public string AttendanceMarkedStatus { get; set; }
    
    public string AttendanceApprovedStatus { get; set; }
}