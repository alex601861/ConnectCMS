using CMSTrain.Application.DTOs.Count;
using CMSTrain.Application.DTOs.Candidate;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.TrainingCandidate;
using CMSTrain.Application.DTOs.ClientOrganization;

namespace CMSTrain.Controllers;

[Route("api/training-candidate")]
public class TrainingCandidateController(ITrainingCandidateService trainingCandidateService) : BaseController<TrainingCandidateController>
{
    [HttpGet("assignment-details/{trainingCandidateId:guid}")]
    public IActionResult GetTrainingCandidateAssignmentDetails(Guid trainingCandidateId)
    {
        var result = trainingCandidateService.GetTrainingCandidateAssignmentDetails(trainingCandidateId);
        
        return Ok(new ResponseDto<TrainingCandidateAssignmentDetailsDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training candidate details successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("approved-assignment-details/{trainingCandidateId:guid}")]
    public IActionResult GetApprovedTrainingCandidateAssignmentDetails(Guid trainingCandidateId)
    {
        var result = trainingCandidateService.GetApprovedTrainingCandidateAssignmentDetails(trainingCandidateId);
        
        return Ok(new ResponseDto<GetAllTrainingRequestsForAdmin>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training candidate details successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("training-assignment-details/{trainingId:guid}")]
    public IActionResult GetTrainingCandidateAssignmentDetailsForTraining(Guid trainingId)
    {
        var result = trainingCandidateService.GetTrainingCandidateAssignmentDetailsForTraining(trainingId);
        
        return Ok(new ResponseDto<TrainingCandidateAssignmentDetailsDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training candidate details successfully retrieved.",
            Result = result
        });
    }
    
    [HttpPost("self-request")]
    public IActionResult SelfCandidateAssignment(SelfCandidateAssignmentDto assignment)
    {
        trainingCandidateService.SelfCandidateAssignment(assignment);
        
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your request has been successfully sent.",
            Result = true
        });
    }
    
    [HttpPost("client-request")]
    public IActionResult ClientCandidateAssignment(ClientCandidateAssignmentDto assignment)
    {
        trainingCandidateService.ClientCandidateAssignment(assignment);
        
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your candidate's request has been successfully sent.",
            Result = true
        });
    }

    [HttpPost("admin-request")]
    public IActionResult CandidateAssignment(AssignCandidatesDto unAssigned)
    {
        trainingCandidateService.AdminCandidateAssignment(unAssigned);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your candidate's request has been successfully sent.",
            Result = true
        });
    }

    [HttpPost("approve-reject")]
    public IActionResult ApprovalRejectTrainingCandidateRequest(ApproveRejectRequestDto approveRejectRequest)
    {
        trainingCandidateService.ApprovalRejectTrainingCandidateRequest(approveRejectRequest);
        
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your action on approval request has been successfully completed.",
            Result = true
        });
    }
    
    [HttpDelete("{trainingCandidateId:guid}")]
    public IActionResult RemoveCandidateFromTraining(Guid trainingCandidateId)
    {
        trainingCandidateService.RemoveCandidateFromTraining(trainingCandidateId);
        
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The respective candidate has been successfully removed from the training.",
            Result = true
        });
    }
    
    [HttpPatch("handle-organizational-permission/{trainingCandidateId:guid}")]
    public IActionResult HandleOrganizationCandidatesPermission(Guid trainingCandidateId)
    {
        trainingCandidateService.HandleOrganizationCandidatesPermission(trainingCandidateId);
        
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your training permission request has been successfully updated.",
            Result = true
        });
    }
    
    [HttpDelete("cancel/{trainingCandidateId:guid}")]
    public IActionResult CancelTrainingRequest(Guid trainingCandidateId)
    {
        trainingCandidateService.CancelTrainingRequest(trainingCandidateId);
        
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your training request has been successfully cancelled.",
            Result = true
        });
    }
    
    [HttpGet("summary")]
    public IActionResult GetTrainingRequestsCount(Guid? trainingId)
    {
        var result = trainingCandidateService.GetTrainingRequestsCount(trainingId);

        return Ok(new ResponseDto<GetTrainingRequestsCount>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training candidate requests successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("approval-matrix-count")]
    public IActionResult GetApprovalMatrixCount(Guid? trainingId)
    {
        var result = trainingCandidateService.GetApprovalMatrixCount(trainingId);

        return Ok(new ResponseDto<ApprovalMatrixCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Approval matrix count successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("requests/admin/{request:int}")]
    public IActionResult GetAllTrainingRequestsForAdmin(int request, int pageNumber, int pageSize, string? search, Guid? trainingId)
    {
        var result = trainingCandidateService.GetAllTrainingRequestsForAdmin(request, pageNumber, pageSize, out var rowCount, search, trainingId);
        
        return Ok(new CollectionDto<GetAllTrainingRequestsForAdmin>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training requests successfully retrieved.",
        });
    }
    
    [HttpGet("requests/admin/list/{request:int}")]
    public IActionResult GetAllTrainingRequestsForAdmin(int request, string? search, Guid? trainingId)
    {
        var result = trainingCandidateService.GetAllTrainingRequestsForAdmin(request, search, trainingId);
        
        return Ok(new ResponseDto<List<GetAllTrainingRequestsForAdmin>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training requests successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("requests/candidate/{request:int}")]
    public IActionResult GetAllTrainingRequestsForCandidate(int request, int pageNumber, int pageSize, string? search)
    {
        var result = trainingCandidateService.GetAllTrainingRequestsForCandidate(request, pageNumber, pageSize, out var rowCount, search);
        
        return Ok(new CollectionDto<GetAllTrainingRequestsForCandidate>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training requests successfully retrieved.",
        });
    }
    
    [HttpGet("requests/candidate/list/{request:int}")]
    public IActionResult GetAllTrainingRequestsForCandidate(int request, string? search)
    {
        var result = trainingCandidateService.GetAllTrainingRequestsForCandidate(request, search);
        
        return Ok(new ResponseDto<List<GetAllTrainingRequestsForCandidate>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training requests successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("requests/candidate/count")]
    public IActionResult GetTrainingRequestCountsForCandidate()
    {
        var result = trainingCandidateService.GetTrainingRequestCountsForCandidate();

        return Ok(new ResponseDto<ApprovalMatrixCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available request training count successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("approved-candidates/{trainingId:guid}")]
    public IActionResult GetAllApprovedCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize, string? search)
    {
        var result = trainingCandidateService.GetAllApprovedCandidatesForTraining(trainingId, pageNumber, pageSize, out var rowCount, search);
        
        return Ok(new CollectionDto<GetApprovedCandidateDetailsDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Approved candidates successfully retrieved.",
        });
    }

    [HttpGet("unassigned-candidates/{trainingId:guid}")]
    public IActionResult GetAllUnassignedCandidatesForTraining(Guid trainingId)
    {
        var result = trainingCandidateService.GetAllUnassignedCandidatesForTraining(trainingId);

        return Ok(new ResponseDto<List<GetCandidateDetailsDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Unassigned candidates successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("unassigned/client/candidates/{trainingId:guid}")]
    public IActionResult GetAllUnassignedClientCandidatesForTraining(Guid trainingId)
    {
        var result = trainingCandidateService.GetAllUnassignedClientCandidatesForTraining(trainingId);

        return Ok(new ResponseDto<List<GetCandidateDetailsDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Unassigned candidates for client successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("approved-candidates/list/{trainingId:guid}")]
    public IActionResult GetAllApprovedCandidatesForTraining(Guid trainingId)
    {
        var result = trainingCandidateService.GetAllApprovedCandidatesForTraining(trainingId);
        
        return Ok(new ResponseDto<List<GetApprovedCandidateDetailsDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Approved candidates successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("colleague-candidates/{trainingId:guid}")]
    public IActionResult GetAllColleagueCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize, string? search)
    {
        var result = trainingCandidateService.GetAllColleagueCandidatesForTraining(trainingId, pageNumber, pageSize, out var rowCount, search);
        
        return Ok(new CollectionDto<GetApprovedCandidateDetailsDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Colleague candidates successfully retrieved.",
        });
    }
    
    [HttpGet("colleague-candidates/list/{trainingId:guid}")]
    public IActionResult GetAllColleagueCandidatesForTraining(Guid trainingId, string? search)
    {
        var result = trainingCandidateService.GetAllColleagueCandidatesForTraining(trainingId, search);
        
        return Ok(new ResponseDto<List<GetApprovedCandidateDetailsDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Colleague candidates successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("organizational-candidates/{trainingId:guid}")]
    public IActionResult GetAllOrganizationalCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize, string? search)
    {
        var result = trainingCandidateService.GetAllOrganizationalCandidatesForTraining(trainingId, pageNumber, pageSize, out var rowCount, search);
        
        return Ok(new CollectionDto<GetApprovedCandidateDetailsDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Organizational candidates successfully retrieved.",
        });
    }
    
    [HttpGet("organizational-candidates/list/{trainingId:guid}")]
    public IActionResult GetAllOrganizationalCandidatesForTraining(Guid trainingId, string? search)
    {
        var result = trainingCandidateService.GetAllOrganizationalCandidatesForTraining(trainingId, search);
        
        return Ok(new ResponseDto<List<GetApprovedCandidateDetailsDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Organizational candidates successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("client-candidates/{trainingId:guid}")]
    public IActionResult GetAllAssignedCandidatesForClient(Guid trainingId)
    {
        var result = trainingCandidateService.GetAllAssignedCandidatesForClient(trainingId);
        
        return Ok(new ResponseDto<List<GetApprovedCandidateDetailsDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Client candidates successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("client-candidates/list/{trainingId:guid}")]
    public IActionResult GetAllAssignedCandidatesForClient(Guid trainingId, int pageNumber, int pageSize)
    {
        var result = trainingCandidateService.GetAllAssignedCandidatesForClient(trainingId, pageNumber, pageSize, out var rowCount);
        
        return Ok(new CollectionDto<GetApprovedCandidateDetailsDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Client candidates successfully retrieved.",
        });
    }
    
    [HttpGet("client-candidates/{trainingId:guid}/{request:int}")]
    public IActionResult GetAllClientCandidatesForTraining(Guid trainingId, int request)
    {
        var result = trainingCandidateService.GetAllClientCandidatesForTraining(trainingId, request);

        return Ok(new ResponseDto<List<GetClientOrganizationUsersDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Associated candidates successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("client-candidates-count/{trainingId:guid}")]
    public IActionResult GetClientOrganizationCandidateCount(Guid trainingId)
    {
        var result = trainingCandidateService.ClientOrganizationNominationsCount(trainingId);

        return Ok(new ResponseDto<GetClientOrganizationCandidateCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Result = result
        });
    }
}
