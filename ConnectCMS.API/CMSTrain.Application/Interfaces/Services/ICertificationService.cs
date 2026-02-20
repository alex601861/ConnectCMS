using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Certification;

namespace CMSTrain.Application.Interfaces.Services;

public interface ICertificationService : ITransientService
{
    GetCertificationDetails GetCertificationDetailsById(Guid certificationId);

    GetCertificationDetails GetCertificationDetailsByTrainingId(Guid trainingId);

    GetCertificationDetails GetCertificationDetailsByTrainingCandidateId(Guid trainingCandidateId);
    
    void IssueTrainingCandidateCertification(IssueCertificationDto issueCertification);

    void IssueTrainingCertifications(Guid trainingId);
}