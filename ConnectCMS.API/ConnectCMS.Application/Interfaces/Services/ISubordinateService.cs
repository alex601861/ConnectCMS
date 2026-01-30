using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Candidate;
using CMSTrain.Application.DTOs.Subordinate;

namespace CMSTrain.Application.Interfaces.Services;

public interface ISubordinateService : ITransientService
{
    GetSubordinateDto GetSubordinateById(Guid subordinateId);

    GetCandidateDetailsDto GetCandidateBySubordinateId(Guid subordinateId);
    
    List<GetSubordinateDto> GetSubordinateDetails(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null, int? type = null);

    List<GetSubordinateDto> GetSubordinateDetails(Guid trainingId);

    List<GetSubordinateDto> GetSubordinateDetailsForTrainingCandidate(Guid trainingCandidateId, int pageNumber, int pageSize, out int rowCount);

    List<GetSubordinateDto> GetSubordinateDetailsForTrainingCandidate(Guid trainingCandidateId);

    GetSubordinateDto GetSubordinateDetails(Guid trainingId, SubordinateType subordinateType);

    void InsertSubordinateForCandidates(CreateSubordinateDto subordinate);

    void InsertSubordinateForCandidates(CreateClientSubordinateDto subordinate);
}
