using CMSTrain.Application.Common.Response;
using CMSTrain.Application.DTOs.Certification;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/certification")]
public class CertificationController(ICertificationService certificationService) : BaseController<CertificationController>
{
    [HttpGet("{certificationId:guid}")]
    public IActionResult GetCertificationDetailsById(Guid certificationId)
    {
        var certificationDetails = certificationService.GetCertificationDetailsById(certificationId);

        return Ok(new ResponseDto<GetCertificationDetails>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Certification successfully retrieved.",
            Result = certificationDetails
        });
    }
    
    [HttpGet("training/{trainingId:guid}")]
    public IActionResult GetCertificationDetailsByTrainingId(Guid trainingId)
    {
        var certificationDetails = certificationService.GetCertificationDetailsByTrainingId(trainingId);

        return Ok(new ResponseDto<GetCertificationDetails>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Certification successfully retrieved.",
            Result = certificationDetails
        });
    }
    
    [HttpGet("training-candidate/{trainingCandidateId:guid}")]
    public IActionResult GetCertificationDetailsByTrainingCandidateId(Guid trainingCandidateId)
    {
        var certificationDetails = certificationService.GetCertificationDetailsByTrainingCandidateId(trainingCandidateId);

        return Ok(new ResponseDto<GetCertificationDetails>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Certification successfully retrieved.",
            Result = certificationDetails
        });
    }
    
    [HttpPost]
    public IActionResult IssueTrainingCandidateCertification(IssueCertificationDto issueCertification)
    {
        certificationService.IssueTrainingCandidateCertification(issueCertification);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Certification successfully issues / registered.",
            Result = true
        });
    }
}