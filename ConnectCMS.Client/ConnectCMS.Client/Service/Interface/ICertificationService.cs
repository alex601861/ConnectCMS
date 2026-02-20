using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Certification;
using CMSTrain.Client.Models.Responses.Certification;

namespace CMSTrain.Client.Service.Interface;

public interface ICertificationService : ITransientService
{
    Task<ResponseDto<GetCertificationDetails?>?> GetCertificationDetailsById(Guid certificationId);

    Task<ResponseDto<GetCertificationDetails?>?> GetCertificationDetailsByTrainingId(Guid trainingId);

    Task<ResponseDto<GetCertificationDetails?>?> GetCertificationDetailsByTrainingCandidateId(Guid trainingCandidateId);
    
    Task<ResponseDto<bool?>?> IssueTrainingCandidateCertification(IssueCertificationDto issueCertification);
}