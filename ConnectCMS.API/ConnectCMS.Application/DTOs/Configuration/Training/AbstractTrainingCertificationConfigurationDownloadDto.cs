namespace CMSTrain.Application.DTOs.Configuration.Training;

public class AbstractTrainingCertificationConfigurationDownloadDto : AbstractTrainingCertificationConfigurationDto
{
    public string PrimaryLogo { get; set; }
    
    public string SecondaryLogo { get; set; }
    
    public string Signature { get; set; }
}