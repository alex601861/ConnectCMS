using CMSTrain.Application.DTOs.Candidate;

namespace CMSTrain.Application.DTOs.Attendance;

public class GetAttendanceResponseDto
{
    public Guid? Id { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsActionCompleted { get; set; }

    public bool? IsApproved { get; set; }

    public string? Remarks { get; set; }

    public string? AttendedAt { get; set; }
    
    public string? ActionDate { get; set; }

    public string? ApprovedBy { get; set; }
    
    public string? ApprovalRole { get; set; }
    
    public GetCandidateDetailsDto CandidateDetails { get; set; }
}