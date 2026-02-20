using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Responses.Count;
using CMSTrain.Client.Models.Responses.Candidate;
using CMSTrain.Client.Models.Requests.TrainingCandidate;
using CMSTrain.Client.Models.Responses.TrainingCandidate;
using CMSTrain.Client.Models.Responses.ClientOrganization;

namespace CMSTrain.Client.Service.Interface;

public interface ITrainingCandidateService : ITransientService
{
    Task<ResponseDto<TrainingCandidateAssignmentDetailsDto?>?> GetTrainingCandidateAssignmentDetails(Guid trainingCandidateId);

    Task<ResponseDto<GetAllTrainingRequestsForAdmin?>?> GetApprovedTrainingCandidateAssignmentDetails(Guid trainingCandidateId);

    Task<ResponseDto<TrainingCandidateAssignmentDetailsDto?>?> GetTrainingCandidateAssignmentDetailsForTraining(Guid trainingId);

    Task<ResponseDto<bool?>?> SelfCandidateAssignment(SelfCandidateAssignmentDto assignment);

    Task<ResponseDto<bool?>?> ClientCandidateAssignment(ClientCandidateAssignmentDto assignment);
    
    Task<ResponseDto<bool?>?> AdminCandidateAssignment(AssignCandidatesDto candidate);

    Task<ResponseDto<bool?>?> ApprovalRejectTrainingCandidateRequest(ApproveRejectRequestDto approveRejectRequest);

    Task<ResponseDto<bool?>?> RemoveCandidateFromTraining(Guid trainingCandidateId);

    Task<ResponseDto<bool?>?> CancelTrainingRequest(Guid trainingCandidateId);

    Task<ResponseDto<GetTrainingRequestsCount?>?> GetTrainingRequestsCount(Guid? trainingId = null);

    Task<ResponseDto<ApprovalMatrixCountDto?>?> GetApprovalMatrixCount(Guid? trainingId = null);
    
    Task<CollectionDto<GetAllTrainingRequestsForAdmin>?> GetAllTrainingRequestsForAdmin(int action, int pageNumber, int pageSize, string? search = null, Guid? trainingId = null);

    Task<ResponseDto<List<GetAllTrainingRequestsForAdmin>?>?> GetAllTrainingRequestsForAdmin(int action, string? search = null, Guid? trainingId = null);

    Task<CollectionDto<GetAllTrainingRequestsForCandidate>?> GetAllTrainingRequestsForCandidate(int action, int pageNumber, int pageSize, string? search = null);
    
    Task<ResponseDto<List<GetAllTrainingRequestsForCandidate>?>?> GetAllTrainingRequestsForCandidate(int action, string? search = null);

    Task<ResponseDto<ApprovalMatrixCountDto?>?> GetTrainingRequestCountsForCandidate();

    Task<CollectionDto<GetApprovedCandidateDetailsDto>?> GetAllApprovedCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize,  string? search = null);
    
    Task<ResponseDto<List<GetApprovedCandidateDetailsDto>?>?> GetAllApprovedCandidatesForTraining(Guid trainingId);

    Task<ResponseDto<List<GetCandidateDetailsDto>?>?> GetAllUnassignedCandidatesForTraining(Guid trainingId);

    Task<CollectionDto<GetApprovedCandidateDetailsDto>?> GetAllColleagueCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize, string? search = null);

    Task<ResponseDto<List<GetApprovedCandidateDetailsDto>?>?> GetAllColleagueCandidatesForTraining(Guid trainingId);

    Task<CollectionDto<GetApprovedCandidateDetailsDto>?> GetAllOrganizationalCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize,  string? search = null);

    Task<ResponseDto<List<GetApprovedCandidateDetailsDto>?>?> GetAllOrganizationalCandidatesForTraining(Guid trainingId);

    Task<CollectionDto<GetApprovedCandidateDetailsDto>?> GetAllAssignedCandidatesForClient(Guid trainingId, int pageNumber, int pageSize);

    Task<ResponseDto<List<GetApprovedCandidateDetailsDto>?>?> GetAllAssignedCandidatesForClient(Guid trainingId);
    
    Task<ResponseDto<List<GetCandidateDetailsDto>?>?> GetAllUnassignedClientCandidatesForTraining(Guid trainingId);

    Task<ResponseDto<List<GetClientOrganizationUsersDto>?>?> GetAllClientCandidatesForTraining(Guid trainingId, int requestAction);

    Task<ResponseDto<GetClientOrganizationCandidateCountDto?>?> GetClientOrganizationCandidateCount(Guid trainingId);
}