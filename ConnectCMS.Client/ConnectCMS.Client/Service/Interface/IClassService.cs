using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Requests.Class;
using CMSTrain.Client.Models.Responses.Class;
using CMSTrain.Client.Models.Responses.Count;
using CMSTrain.Client.Models.Responses.Country;
using CMSTrain.Client.Service.Dependency;

namespace CMSTrain.Client.Service.Interface;

public interface IClassService : ITransientService
{
    Task<CollectionDto<GetClassDto>?> GetAllClasses(Guid trainingId, int pageNumber, int pageSize, string? search = null, int? status = null);
    
    Task<ResponseDto<List<GetClassDto>?>?> GetAllClasses(Guid trainingId);

    Task<CollectionDto<GetClassForTrainersDto>?> GetAllClassesForTrainers(Guid trainingId, int pageNumber, int pageSize, string? search = null, int? status = null);

    Task<ResponseDto<List<GetClassForTrainersDto>?>?> GetAllClassesForTrainers(Guid trainingId);

    Task<CollectionDto<GetClassForCandidatesDto>?> GetAllClassesForCandidates(Guid trainingId, int pageNumber, int pageSize, string? search = null, int? status = null);

    Task<ResponseDto<List<GetClassForCandidatesDto>?>?> GetAllClassesForCandidates(Guid trainingId);

    Task<CollectionDto<GetClassForCandidatesDto>?> GetAllCandidateClasses(Guid trainingCandidateId, int pageNumber, int pageSize);

    Task<ResponseDto<List<GetClassForCandidatesDto>?>?> GetAllCandidateClasses(Guid trainingCandidateId);

    Task<ResponseDto<GetClassDto?>?> GetClassById(Guid classId);
    
    Task<ResponseDto<bool?>?> InsertClass(CreateClassDto @class);

    Task<ResponseDto<bool?>?> UpdateClass(UpdateClassDto @class);

    Task<ResponseDto<bool?>?> DeleteClass(Guid classId);

    Task<ResponseDto<ClassCountDto?>?> GetClassCount(Guid classId);

    Task<ResponseDto<ClassCountDto?>?> GetClassCountForCandidate(Guid classId);

    Task<ResponseDto<ClassCountDto?>?> GetClassDetailsCountForClient(Guid classId);
    
    Task<ResponseDto<ClassCountDto?>?> GetClassDetailsCountForTrainer(Guid classId);
}