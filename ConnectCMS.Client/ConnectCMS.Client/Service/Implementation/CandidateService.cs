using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Responses.Candidate;

namespace CMSTrain.Client.Service.Implementation;

public class CandidateService(IBaseService baseService) : ICandidateService 
{
    public async Task<ResponseDto<GetCandidateDetailsDto?>?> GetCandidateDetailsById(Guid candidateId)
    {
        var pathParameter = new List<string>
        {
            candidateId.ToString()
        };
        
        var response = await baseService.GetAsync<GetCandidateDetailsDto?>(ApiEndpoints.Candidate.GetCandidateDetailsById, pathParameter);

        return response;
    }
}