using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Models.Requests.Configuration.Training;

public class AbstractTrainingCertificationConfigurationUploadDto : AbstractTrainingCertificationConfigurationDto
{
    public IBrowserFile? PrimaryLogo { get; set; }
    
    public IBrowserFile? SecondaryLogo { get; set; }
    
    public IBrowserFile? Signature { get; set; }
}