using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Attendance;
using CMSTrain.Client.Models.Responses.Attendance;

namespace CMSTrain.Client.Service.Interface;

public interface IAttendanceService : ITransientService
{
    Task<CollectionDto<GetAttendanceResponseDto>?> GetAllAttendanceRequests(Guid classId, int pageNumber, int pageSize, string? search = null, bool? isApproved = null);

    Task<ResponseDto<List<GetAttendanceResponseDto>?>?> GetAllAttendanceRequests(Guid classId, string? search = null, bool? isApproved = null);
    
    Task<ResponseDto<GetAttendanceResponseDto?>?> GetAttendanceRequestForCandidate(Guid classId);

    Task<CollectionDto<GetAttendanceResponseDto>?> GetAttendanceRequestForClient(Guid classId, int pageNumber, int pageSize, string? search = null, bool? isApproved = null);

    Task<ResponseDto<List<GetAttendanceResponseDto>?>?> GetAttendanceRequestForClient(Guid classId, string? search = null, bool? isApproved = null);

    Task<ResponseDto<bool?>?> UploadAttendance(AttendanceRequestDto attendanceRequest);
    
    Task<ResponseDto<bool?>?> CancelAttendance(Guid classId);

    Task<ResponseDto<bool?>?> ApproveRejectAttendance(AttendanceApproveRejectDto approveReject);

    Task<ResponseDto<bool?>?> DownloadAttendanceImage(Guid attendanceId);
    
    Task<ResponseDto<bool?>?> ExportAttendanceDetails(Guid classId, Guid? organizationId);
}