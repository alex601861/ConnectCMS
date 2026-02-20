namespace CMSTrain.Application.DTOs.Attendance;

public class AttendanceRequestDto
{
    public Guid ClassId { get; set; }

    public string Attendance { get; set; }
}
