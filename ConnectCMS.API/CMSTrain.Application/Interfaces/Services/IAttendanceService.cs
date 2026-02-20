using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Attendance;
using CMSTrain.Application.DTOs.Class;

namespace CMSTrain.Application.Interfaces.Services;

public interface IAttendanceService : ITransientService
{
    List<GetAttendanceResponseDto> GetAllAttendanceRequests(Guid classId, int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isApproved = null);

    List<GetAttendanceResponseDto> GetAllAttendanceRequests(Guid classId, string? search = null, bool? isApproved = null);
    
    GetAttendanceResponseDto? GetAttendanceRequestForCandidate(Guid classId);

    List<GetAttendanceResponseDto> GetAttendanceRequestForClient(Guid classId, int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isApproved = null);

    List<GetAttendanceResponseDto> GetAttendanceRequestForClient(Guid classId, string? search = null, bool? isApproved = null);

    void UploadAttendance(AttendanceRequestDto attendanceRequest);

    void CancelAttendance(Guid classId);

    void ApproveRejectAttendance(AttendanceApproveRejectDto approveReject);

    string DownloadAttendanceFile(Guid attendanceId);
    
    byte[] ExportAttendanceDetails(Guid classId, Guid? organizationId);
}
