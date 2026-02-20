using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Responses.Candidate;

namespace CMSTrain.Client.Service.Interface;

public interface ICandidateService : ITransientService
{
    Task<ResponseDto<GetCandidateDetailsDto?>?> GetCandidateDetailsById(Guid candidateId);
}