using Microsoft.AspNetCore.Http;

namespace CMSTrain.Application.DTOs.Configuration.Training;

public class AbstractTrainingCertificationConfigurationUploadDto : AbstractTrainingCertificationConfigurationDto
{
    public IFormFile? PrimaryLogo { get; set; }
    
    public IFormFile? SecondaryLogo { get; set; }
    
    public IFormFile? Signature { get; set; }
}