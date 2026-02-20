using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Responses.Count;
using CMSTrain.Client.Models.Responses.Candidate;
using CMSTrain.Client.Models.Requests.TrainingCandidate;
using CMSTrain.Client.Models.Responses.TrainingCandidate;
using CMSTrain.Client.Models.Responses.ClientOrganization;

namespace CMSTrain.Client.Service.Implementation;

public class TrainingCandidateService(IBaseService baseService) : ITrainingCandidateService
{
    public async Task<ResponseDto<TrainingCandidateAssignmentDetailsDto?>?> GetTrainingCandidateAssignmentDetails(Guid trainingCandidateId)
    {
        var pathParameter = new List<string>
        {
            trainingCandidateId.ToString()
        };
        
        var response = await baseService.GetAsync<TrainingCandidateAssignmentDetailsDto?>(ApiEndpoints.TrainingCandidate.GetTrainingCandidateAssignmentDetails, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetAllTrainingRequestsForAdmin?>?> GetApprovedTrainingCandidateAssignmentDetails(Guid trainingCandidateId)
    {
        var pathParameter = new List<string>
        {
            trainingCandidateId.ToString()
        };
        
        var response = await baseService.GetAsync<GetAllTrainingRequestsForAdmin?>(ApiEndpoints.TrainingCandidate.GetApprovedTrainingCandidateAssignmentDetails, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<TrainingCandidateAssignmentDetailsDto?>?> GetTrainingCandidateAssignmentDetailsForTraining(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<TrainingCandidateAssignmentDetailsDto?>(ApiEndpoints.TrainingCandidate.GetTrainingCandidateAssignmentDetailsForTraining, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> SelfCandidateAssignment(SelfCandidateAssignmentDto assignment)
    {
        var jsonRequest = JsonSerializer.Serialize(assignment);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.TrainingCandidate.SelfCandidateAssignment, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> ClientCandidateAssignment(ClientCandidateAssignmentDto assignment)
    {
        var jsonRequest = JsonSerializer.Serialize(assignment);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.TrainingCandidate.ClientCandidateAssignment, content);
        
        return response;
    }
    
    public async Task<ResponseDto<bool?>?> AdminCandidateAssignment(AssignCandidatesDto candidate)
    {
        var jsonRequest = JsonSerializer.Serialize(candidate);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8,"application/json");

        var response = await baseService.PostAsync<bool?>(ApiEndpoints.TrainingCandidate.AdminCandidateAssignment, content);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> ApprovalRejectTrainingCandidateRequest(ApproveRejectRequestDto approveRejectRequest)
    {
        var jsonRequest = JsonSerializer.Serialize(approveRejectRequest);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.TrainingCandidate.ApprovalRejectTrainingCandidateRequest, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> RemoveCandidateFromTraining(Guid trainingCandidateId)
    {
        var pathParameter = new List<string>
        {
            trainingCandidateId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.TrainingCandidate.RemoveCandidateFromTraining, Constants.DeleteType.Delete, pathParameter);
        
        return response;
    }
    
    public async Task<ResponseDto<bool?>?> CancelTrainingRequest(Guid trainingCandidateId)
    {
        var pathParameter = new List<string>
        {
            trainingCandidateId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.TrainingCandidate.CancelTrainingRequest, Constants.DeleteType.Delete, pathParameter);
        
        return response;
    }

    public async Task<ResponseDto<GetTrainingRequestsCount?>?> GetTrainingRequestsCount(Guid? trainingId = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "trainingId", trainingId.ToString() }
        };

        var response = await baseService.GetAsync<GetTrainingRequestsCount?>(ApiEndpoints.TrainingCandidate.GetTrainingRequestsCount, parameters: queryParameter);

        return response;
    }

    public async Task<ResponseDto<ApprovalMatrixCountDto?>?> GetApprovalMatrixCount(Guid? trainingId = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "trainingId", trainingId.ToString() }
        };
        
        var response = await baseService.GetAsync<ApprovalMatrixCountDto?>(ApiEndpoints.TrainingCandidate.GetApprovalMatrixCount, parameters: queryParameter);

        return response;
    }
    
    public async Task<CollectionDto<GetAllTrainingRequestsForAdmin>?> GetAllTrainingRequestsForAdmin(int action, int pageNumber, int pageSize, string? search = null, Guid? trainingId = null)
    {
        var pathParameter = new List<string>
        {
            action.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "trainingId", trainingId.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetAllTrainingRequestsForAdmin>(ApiEndpoints.TrainingCandidate.GetAllTrainingRequestsForAdmin, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetAllTrainingRequestsForAdmin>?>?> GetAllTrainingRequestsForAdmin(int action, string? search = null, Guid? trainingId = null)
    {
        var pathParameter = new List<string>
        {
            action.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "search", search },
            { "trainingId", trainingId?.ToString() }
        };
        
        var response = await baseService.GetAsync<List<GetAllTrainingRequestsForAdmin>?>(ApiEndpoints.TrainingCandidate.GetAllTrainingRequestsForAdminList, pathParameter, queryParameter);

        return response;
    }

    public async Task<CollectionDto<GetAllTrainingRequestsForCandidate>?> GetAllTrainingRequestsForCandidate(int action, int pageNumber, int pageSize, string? search = null)
    {
        var pathParameter = new List<string>
        {
            action.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search }
        };
        
        var response = await baseService.GetPagedAsync<GetAllTrainingRequestsForCandidate>(ApiEndpoints.TrainingCandidate.GetAllTrainingRequestsForCandidate, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetAllTrainingRequestsForCandidate>?>?> GetAllTrainingRequestsForCandidate(int action, string? search = null)
    {
        var pathParameter = new List<string>
        {
            action.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "search", search }
        };
        
        var response = await baseService.GetAsync<List<GetAllTrainingRequestsForCandidate>?>(ApiEndpoints.TrainingCandidate.GetAllTrainingRequestsForCandidateList, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<ApprovalMatrixCountDto?>?> GetTrainingRequestCountsForCandidate()
    {
        var response = await baseService.GetAsync<ApprovalMatrixCountDto?>(ApiEndpoints.TrainingCandidate.GetTrainingRequestCountsForCandidate);

        return response;
    }
    
    public async Task<CollectionDto<GetApprovedCandidateDetailsDto>?> GetAllApprovedCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize, string? search = null)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
        };
        
        var response = await baseService.GetPagedAsync<GetApprovedCandidateDetailsDto>(ApiEndpoints.TrainingCandidate.GetAllApprovedCandidatesForTraining, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetApprovedCandidateDetailsDto>?>?> GetAllApprovedCandidatesForTraining(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetApprovedCandidateDetailsDto>?>(ApiEndpoints.TrainingCandidate.GetAllApprovedCandidatesForTrainingList, pathParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetCandidateDetailsDto>?>?> GetAllUnassignedCandidatesForTraining(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };

        var response = await baseService.GetAsync<List<GetCandidateDetailsDto>>(ApiEndpoints.TrainingCandidate.GetUnassignedCandidateForTraining, pathParameter);

        return response;
    }
    
    public async Task<CollectionDto<GetApprovedCandidateDetailsDto>?> GetAllColleagueCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize, string? search = null)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
        };
        
        var response = await baseService.GetPagedAsync<GetApprovedCandidateDetailsDto>(ApiEndpoints.TrainingCandidate.GetAllColleagueCandidatesForTraining, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetApprovedCandidateDetailsDto>?>?> GetAllColleagueCandidatesForTraining(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetApprovedCandidateDetailsDto>?>(ApiEndpoints.TrainingCandidate.GetAllColleagueCandidatesForTrainingList, pathParameter);

        return response;
    }

    public async Task<CollectionDto<GetApprovedCandidateDetailsDto>?> GetAllOrganizationalCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize, string? search = null)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
        };
        
        var response = await baseService.GetPagedAsync<GetApprovedCandidateDetailsDto>(ApiEndpoints.TrainingCandidate.GetAllOrganizationalCandidatesForTraining, pathParameter, queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetApprovedCandidateDetailsDto>?>?> GetAllOrganizationalCandidatesForTraining(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetApprovedCandidateDetailsDto>?>(ApiEndpoints.TrainingCandidate.GetAllOrganizationalCandidatesForTrainingList, pathParameter);

        return response;
    }

    public async Task<CollectionDto<GetApprovedCandidateDetailsDto>?> GetAllAssignedCandidatesForClient(Guid trainingId, int pageNumber, int pageSize)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetApprovedCandidateDetailsDto>(ApiEndpoints.TrainingCandidate.GetAllAssignedCandidatesForClient, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetApprovedCandidateDetailsDto>?>?> GetAllAssignedCandidatesForClient(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetApprovedCandidateDetailsDto>?>(ApiEndpoints.TrainingCandidate.GetAllAssignedCandidatesForClientList, pathParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetCandidateDetailsDto>?>?> GetAllUnassignedClientCandidatesForTraining(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };

        var response = await baseService.GetAsync<List<GetCandidateDetailsDto>>(ApiEndpoints.TrainingCandidate.GetUnassignedClientCandidateForTraining, pathParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetClientOrganizationUsersDto>?>?> GetAllClientCandidatesForTraining(Guid trainingId, int requestAction)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString(),
            requestAction.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetClientOrganizationUsersDto>?>(ApiEndpoints.TrainingCandidate.GetAllClientCandidatesForTraining, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<GetClientOrganizationCandidateCountDto?>?> GetClientOrganizationCandidateCount(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<GetClientOrganizationCandidateCountDto>(ApiEndpoints.TrainingCandidate.GetClientOrganizationForCandidateCount, pathParameter);

        return response;
    }
}