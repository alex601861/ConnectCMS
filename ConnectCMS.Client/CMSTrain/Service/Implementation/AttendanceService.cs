using System.Text.Json;
using Microsoft.JSInterop;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Attendance;
using CMSTrain.Client.Models.Responses.Attendance;
using CMSTrain.Client.Models.Responses.Class;
using CMSTrain.Client.Models.Responses.Organization;

namespace CMSTrain.Client.Service.Implementation;

public class AttendanceService(IBaseService baseService, IJSRuntime jsRuntime) : IAttendanceService
{
    public async Task<CollectionDto<GetAttendanceResponseDto>?> GetAllAttendanceRequests(Guid classId, int pageNumber, int pageSize, string? search = null, bool? isApproved = null)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "isApproved", isApproved?.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetAttendanceResponseDto>(endpoint: ApiEndpoints.Attendance.GetAllAttendanceRequests, path: pathParameter, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetAttendanceResponseDto>?>?> GetAllAttendanceRequests(Guid classId, string? search = null, bool? isApproved = null)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>
        {
            { "search", search },
            { "isApproved", isApproved?.ToString() }
        };
        
        var response = await baseService.GetAsync<List<GetAttendanceResponseDto>?>(ApiEndpoints.Attendance.GetAllAttendanceRequestsList, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<GetAttendanceResponseDto?>?> GetAttendanceRequestForCandidate(Guid classId)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };
        
        var response = await baseService.GetAsync<GetAttendanceResponseDto?>(ApiEndpoints.Attendance.GetAttendanceRequestForCandidate, pathParameter);

        return response;
    }
    
    public async Task<CollectionDto<GetAttendanceResponseDto>?> GetAttendanceRequestForClient(Guid classId, int pageNumber, int pageSize, string? search = null, bool? isApproved = null)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "isApproved", isApproved?.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetAttendanceResponseDto>(endpoint: ApiEndpoints.Attendance.GetAttendanceRequestForClient, path: pathParameter, parameters: queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetAttendanceResponseDto>?>?> GetAttendanceRequestForClient(Guid classId, string? search = null, bool? isApproved = null)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>
        {
            { "search", search },
            { "isApproved", isApproved?.ToString() }
        };
        
        var response = await baseService.GetAsync<List<GetAttendanceResponseDto>?>(ApiEndpoints.Attendance.GetAttendanceRequestForClientList, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> UploadAttendance(AttendanceRequestDto attendanceRequest)
    {
        var jsonRequest = JsonSerializer.Serialize(attendanceRequest);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Attendance.UploadAttendance,content);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> ApproveRejectAttendance(AttendanceApproveRejectDto approveReject)
    {
        var jsonRequest = JsonSerializer.Serialize(approveReject);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Attendance.ApproveRejectAttendance,content);

        return response;
    }

    public async Task<ResponseDto<bool?>?> CancelAttendance(Guid classId)
    {
        var pathParameter = new List<string>()
        {
            classId.ToString()
        };

        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Attendance.CancelAttendance, Constants.DeleteType.Delete, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> DownloadAttendanceImage(Guid attendanceId)
    {
        var pathParameter = new List<string>()
        {
            attendanceId.ToString()
        };
        
        var result = await baseService.DownloadAsync(ApiEndpoints.Attendance.DownloadAttendanceImage, pathParameter);

        if (result is not { content: not null, response: not null })
        {
            return new ResponseDto<bool?>()
            {
                Result = false,
                Message = "Attendance could not be downloaded",
                StatusCode = StatusCode.Status400BadRequest
            };
        }
        
        var response = result.response;
        
        var content = result.content;
            
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        await jsRuntime.InvokeVoidAsync("downloadFile", content, $"Attendance - {attendanceId}", contentType);

        return new ResponseDto<bool?>()
        {
            Result = true,
            Message = "Attendance image successfully downloaded.",
            StatusCode = StatusCode.Status200Ok
        };
    }

    public async Task<ResponseDto<bool?>?> ExportAttendanceDetails(Guid classId, Guid? organizationId)
    {
        var pathParameter = new List<string>()
        {
            classId.ToString()
        };

        var organizationPathParameter = new List<string>()
        {
            organizationId?.ToString() ?? ""
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "organizationId", organizationId?.ToString() }
        };
        
        var result = await baseService.DownloadAsync(ApiEndpoints.Attendance.ExportAttendanceDetails, pathParameter, queryParameter);

        var organization = organizationId != null 
            ? await baseService.GetAsync<GetOrganizationDto?>(ApiEndpoints.Organization.GetOrganizationById, organizationPathParameter)
            : null;

        var @class = await baseService.GetAsync<GetClassDto?>(ApiEndpoints.Class.GetClassById, pathParameter);

        if (result is not { content: not null, response: not null })
        {
            return new ResponseDto<bool?>()
            {
                Result = false,
                Message = "Attendance reports could not be downloaded",
                StatusCode = StatusCode.Status400BadRequest
            };
        }
        
        var response = result.response;
        
        var content = result.content;
            
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        await jsRuntime.InvokeVoidAsync("downloadFile", content, $"{@class?.Result?.Title} - {organization?.Result?.Name ?? "Complete"} Attendance Report", contentType);

        return new ResponseDto<bool?>()
        {
            Result = true,
            Message = "Attendance report successfully downloaded.",
            StatusCode = StatusCode.Status200Ok
        };
    }
}