namespace CMSTrain.Application.DTOs.Class;

public class GetClassForTrainersDto : GetClassDto
{
    public int TotalApprovedCandidates { get; set; }
    
    public int TotalAttendedCandidates { get; set; }
    
    public int TotalAcceptedAttendanceCount { get; set; }
    
    public int TotalPendingAttendanceCount { get; set; }

    public int TotalRejectedAttendanceCount { get; set; }
}
