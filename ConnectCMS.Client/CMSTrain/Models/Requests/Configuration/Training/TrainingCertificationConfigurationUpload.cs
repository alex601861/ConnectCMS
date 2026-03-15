namespace CMSTrain.Client.Models.Requests.Configuration.Training;

public class TrainingCertificationConfigurationUpload
{
    public AbstractTrainingCertificationConfigurationUploadDto Certification { get; set; } = new();
}