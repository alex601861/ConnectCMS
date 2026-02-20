using CMSTrain.Application.DTOs.Count;
using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Candidate;
using CMSTrain.Application.DTOs.TrainingCandidate;
using CMSTrain.Application.DTOs.ClientOrganization;

namespace CMSTrain.Application.Interfaces.Services;

public interface ITrainingCandidateService : ITransientService
{
    TrainingCandidateAssignmentDetailsDto GetTrainingCandidateAssignmentDetails(Guid trainingCandidateId);

    GetAllTrainingRequestsForAdmin GetApprovedTrainingCandidateAssignmentDetails(Guid trainingCandidateId);

    TrainingCandidateAssignmentDetailsDto GetTrainingCandidateAssignmentDetailsForTraining(Guid trainingId);

    void SelfCandidateAssignment(SelfCandidateAssignmentDto assignment);

    void ClientCandidateAssignment(ClientCandidateAssignmentDto assignment);

    void AdminCandidateAssignment(AssignCandidatesDto unAssigned);

    void ApprovalRejectTrainingCandidateRequest(ApproveRejectRequestDto approveRejectRequest);

    void RemoveCandidateFromTraining(Guid trainingCandidateId);

    void HandleOrganizationCandidatesPermission(Guid trainingCandidateId);

    void CancelTrainingRequest(Guid trainingCandidateId);

    GetTrainingRequestsCount GetTrainingRequestsCount(Guid? trainingId = null);
    
    ApprovalMatrixCountDto GetApprovalMatrixCount(Guid? trainingId = null);
    
    List<GetAllTrainingRequestsForAdmin> GetAllTrainingRequestsForAdmin(int action, int pageNumber, int pageSize, out int rowCount, string? search = null, Guid? trainingId = null);
    
    List<GetAllTrainingRequestsForAdmin> GetAllTrainingRequestsForAdmin(int action, string? search = null, Guid? trainingId = null);

    List<GetAllTrainingRequestsForCandidate> GetAllTrainingRequestsForCandidate(int action, int pageNumber, int pageSize, out int rowCount, string? search = null);

    List<GetAllTrainingRequestsForCandidate> GetAllTrainingRequestsForCandidate(int action, string? search = null);

    List<GetApprovedCandidateDetailsDto> GetAllApprovedCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null);

    List<GetCandidateDetailsDto> GetAllUnassignedCandidatesForTraining(Guid trainingId);

    List<GetApprovedCandidateDetailsDto> GetAllApprovedCandidatesForTraining(Guid trainingId);

    List<GetApprovedCandidateDetailsDto> GetAllColleagueCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null);

    List<GetApprovedCandidateDetailsDto> GetAllColleagueCandidatesForTraining(Guid trainingId, string? search = null);

    List<GetApprovedCandidateDetailsDto> GetAllOrganizationalCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null);
    
    List<GetApprovedCandidateDetailsDto> GetAllOrganizationalCandidatesForTraining(Guid trainingId, string? search = null);

    List<GetApprovedCandidateDetailsDto> GetAllAssignedCandidatesForClient(Guid trainingId, int pageNumber, int pageSize, out int rowCount);

    List<GetApprovedCandidateDetailsDto> GetAllAssignedCandidatesForClient(Guid trainingId);

    List<GetAllTrainingsForCandidate> GetAllTrainingsForCandidate(int requestAction, int pageNumber, int pageSize, out int rowCount, string? search);
    
    List<GetAllTrainingsForCandidate> GetAllTrainingsForCandidate(int requestAction, string? search);
    
    AvailableTrainingCountDto GetAllAvailableTrainingCountsForCandidate();
    
    List<GetAllTrainingsForCandidate> GetAllAssignedTrainingsForCandidate(int statusAction, int pageNumber, int pageSize, out int rowCount, string? search = null);
    
    List<GetAllTrainingsForCandidate> GetAllAssignedTrainingsForCandidate(int statusAction, string? search = null);

    AssignedTrainingCountDto GetAllAssignedTrainingCountsForCandidate();

    TrainingDetailsCountDto GetTrainingDetailsCountForCandidate(Guid trainingId);

    List<GetAllTrainingsForClient> GetAllTrainingsForClient(int requestAction, int pageNumber, int pageSize, out int rowCount, string? search = null);
    
    List<GetAllTrainingsForClient> GetAllTrainingsForClient(int requestAction, string? search = null);
    
    List<GetCandidateDetailsDto> GetAllUnassignedClientCandidatesForTraining(Guid trainingId);

    AvailableTrainingCountDto GetAllAvailableTrainingCountsForClient();

    List<GetAllTrainingsForClient> GetAllAssignedTrainingsForClient(int statusAction, int pageNumber, int pageSize, out int rowCount, string? search = null);
    
    List<GetAllTrainingsForClient> GetAllAssignedTrainingsForClient(int statusAction, string? search = null);
    
    AssignedTrainingCountDto GetAllAssignedTrainingCountsForClient();

    TrainingDetailsCountDto GetTrainingDetailsCountForClient(Guid trainingId);

    ApprovalMatrixCountDto GetTrainingRequestCountsForCandidate();

    List<GetClientOrganizationUsersDto> GetAllClientCandidatesForTraining(Guid trainingId, int requestAction);

    GetClientOrganizationCandidateCountDto ClientOrganizationNominationsCount(Guid trainingId);
}
