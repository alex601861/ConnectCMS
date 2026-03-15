namespace CMSTrain.Client.Models.Requests.Attendance;

public class AttendanceRequestDto
{
    public Guid ClassId { get; set; }

    public string Attendance { get; set; }
}
