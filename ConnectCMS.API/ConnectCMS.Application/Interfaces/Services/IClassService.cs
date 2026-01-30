using CMSTrain.Application.DTOs.Class;
using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Count;

namespace CMSTrain.Application.Interfaces.Services;

public interface IClassService : ITransientService
{
    List<GetClassDto> GetAllClasses(Guid trainingId);

    List<GetClassDto> GetAllClasses(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null, int? status = null);

    List<GetClassForTrainersDto> GetAllClassesForTrainers(Guid trainingId);

    List<GetClassForTrainersDto> GetAllClassesForTrainers(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null, int? status = null);
    
    List<GetClassForCandidatesDto> GetAllClassesForCandidates(Guid trainingId);

    List<GetClassForCandidatesDto> GetAllClassesForCandidates(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null, int? status = null);
    
    List<GetClassForCandidatesDto> GetAllCandidateClasses(Guid trainingCandidateId);

    List<GetClassForCandidatesDto> GetAllCandidateClasses(Guid trainingCandidateId, int pageNumber, int pageSize, out int rowCount);

    ClassCountDto GetClassDetailsCountForCandidate(Guid classId);
    
    ClassCountDto GetClassDetailsCountForClient(Guid classId);

    ClassCountDto GetClassDetailsCountForTrainer(Guid classId);

    ClassCountDto GetClassDetailsCountForAdmin(Guid classId);

    GetClassForTrainersDto GetClassById(Guid id);

    void InsertClass(CreateClassDto @class);

    void UpdateClass(UpdateClassDto @class);

    void DeleteClass(Guid id);
}
