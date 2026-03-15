using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Subordinate;
using CMSTrain.Client.Models.Responses.Subordinate;

namespace CMSTrain.Client.Service.Interface;

public interface ISubordinateService : ITransientService
{
    Task<ResponseDto<GetSubordinateDto?>?> GetSubordinateById(Guid subordinateId);

    Task<CollectionDto<GetSubordinateDto>?> GetSubordinateDetails(Guid trainingId, int pageNumber, int pageSize, string? search = null, int? type = null);

    Task<ResponseDto<List<GetSubordinateDto>?>?> GetSubordinateDetails(Guid trainingId);

    Task<CollectionDto<GetSubordinateDto>?> GetSubordinateDetailsForTrainingCandidate(Guid trainingCandidateId, int pageNumber, int pageSize);

    Task<ResponseDto<List<GetSubordinateDto>?>?> GetSubordinateDetailsForTrainingCandidate(Guid trainingCandidateId);

    Task<ResponseDto<GetSubordinateDto?>?> GetSubordinateDetails(Guid trainingId, SubordinateType subordinateType);

    Task<ResponseDto<bool?>?> InsertSubordinateForCandidates(CreateCandidateSubordinateDto candidateSubordinate);

    Task<ResponseDto<bool?>?> InsertSubordinateForCandidates(CreateClientSubordinateDto subordinate);
}