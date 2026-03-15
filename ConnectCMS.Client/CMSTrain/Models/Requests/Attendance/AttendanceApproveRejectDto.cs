namespace CMSTrain.Client.Models.Requests.Attendance;

public class AttendanceApproveRejectDto
{
    public Guid RequestId { get; set; }

    public bool IsApproved { get; set; }

    public string Remarks { get; set; }
}
