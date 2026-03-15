using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Certification;
using CMSTrain.Client.Models.Responses.Certification;

namespace CMSTrain.Client.Service.Implementation;

public class CertificationService(IBaseService baseService) : ICertificationService
{
    public async Task<ResponseDto<GetCertificationDetails?>?> GetCertificationDetailsById(Guid certificationId)
    {
        var pathParameter = new List<string>
        {
            certificationId.ToString()
        };
        
        var response = await baseService.GetAsync<GetCertificationDetails?>(ApiEndpoints.Certification.GetCertificationDetailsById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetCertificationDetails?>?> GetCertificationDetailsByTrainingId(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<GetCertificationDetails?>(ApiEndpoints.Certification.GetCertificationDetailsByTrainingId, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<GetCertificationDetails?>?> GetCertificationDetailsByTrainingCandidateId(Guid trainingCandidateId)
    {
        var pathParameter = new List<string>
        {
            trainingCandidateId.ToString()
        };
        
        var response = await baseService.GetAsync<GetCertificationDetails?>(ApiEndpoints.Certification.GetCertificationDetailsByTrainingCandidateId, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> IssueTrainingCandidateCertification(IssueCertificationDto issueCertification)
    {
        var jsonRequest = JsonSerializer.Serialize(issueCertification);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Certification.IssueTrainingCandidateCertification, content);

        return response;
    }
}