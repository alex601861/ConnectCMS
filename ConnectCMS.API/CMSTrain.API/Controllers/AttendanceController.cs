using CMSTrain.Application.Common.Response;
using CMSTrain.Application.DTOs.Attendance;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/attendance")]
public class AttendanceController(IAttendanceService attendanceService, ICandidateService candidateService, IClassService classService, IOrganizationService organizationService) : BaseController<AttendanceController>
{
    [HttpGet("{classId:guid}")]
    public IActionResult GetAllAttendanceRequest(Guid classId, int pageNumber, int pageSize, string? search, bool? isApproved)
    {
        var result = attendanceService.GetAllAttendanceRequests(classId, pageNumber, pageSize, out var rowCount, search, isApproved);

        return Ok(new CollectionDto<GetAttendanceResponseDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The attendances of provided identifier successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("list/{classId:guid}")]
    public IActionResult GetAllAttendanceRequest(Guid classId, string? search, bool? isApproved)
    {
        var result = attendanceService.GetAllAttendanceRequests(classId, search, isApproved);

        return Ok(new ResponseDto<List<GetAttendanceResponseDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The attendances of provided identifier successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("candidate-details/{classId:guid}")]
    public IActionResult GetAttendanceRequestForCandidate(Guid classId)
    {
        var result = attendanceService.GetAttendanceRequestForCandidate(classId);

        return Ok(new ResponseDto<GetAttendanceResponseDto?>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The attendance requests of provided identifier successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("client-details/{classId:guid}")]
    public IActionResult GetAttendanceRequestForClient(Guid classId, int pageNumber, int pageSize, string? search, bool? isApproved)
    {
        var result = attendanceService.GetAttendanceRequestForClient(classId, pageNumber, pageSize, out var rowCount, search, isApproved);

        return Ok(new CollectionDto<GetAttendanceResponseDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The attendances of provided identifier successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("client-details/list/{classId:guid}")]
    public IActionResult GetAttendanceRequestForClient(Guid classId, string? search, bool? isApproved)
    {
        var result = attendanceService.GetAttendanceRequestForClient(classId, search, isApproved);

        return Ok(new ResponseDto<List<GetAttendanceResponseDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The attendance requests of provided identifier successfully fetched.",
            Result = result
        });
    }
    
    [HttpPost]
    public IActionResult UploadAttendance(AttendanceRequestDto attendanceRequest)
    {
       attendanceService.UploadAttendance(attendanceRequest);

       return Ok(new ResponseDto<bool>
       {
           StatusCode = (int)HttpStatusCode.OK,
           Message = "Attendance successfully marked.",
           Result = true
       });
    }

    [HttpDelete("{classId:guid}")]
    public IActionResult CancelAttendance(Guid classId)
    {
        attendanceService.CancelAttendance(classId);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Attendance successfully canceled.",
            Result = true
        });
    }

    [HttpPost("approve-reject")]
    public IActionResult ApproveRejectAttendance(AttendanceApproveRejectDto approveReject)
    {
        attendanceService.ApproveRejectAttendance(approveReject);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = approveReject.IsApproved ? "Attendance successfully approved." : "Attendance successfully rejected.",
            Result = true
        });
    }

    [HttpGet("download/{attendanceId:guid}")]
    public IActionResult DownloadAttendanceImage(Guid attendanceId)
    {
        var filePath = attendanceService.DownloadAttendanceFile(attendanceId);

        var candidateDetails = candidateService.GetCandidateDetailsByAttendanceId(attendanceId);

        if (string.IsNullOrEmpty(filePath))
        {
            throw new NotFoundException("The attendance image could not be downloaded.");
        }

        var fileName = Path.GetFileName(filePath);

        var contentType = GetContentType(fileName);

        return PhysicalFile(filePath, contentType, candidateDetails.Name);
    }
    
    [HttpGet("report/{classId:guid}")]
    public IActionResult ExportAttendanceDetails(Guid classId, Guid? organizationId)
    {
        var result = attendanceService.ExportAttendanceDetails(classId, organizationId);
        
        var classDetails = classService.GetClassById(classId);

        var organization = organizationId != null ? organizationService.GetOrganizationById(organizationId.Value) : null;
        
        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{classDetails.Title} - {organization?.Name ?? "Complete"} Attendance Report");
    }
}
