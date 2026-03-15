namespace CMSTrain.Client.Models.Requests.Resource;

public class TrainingResourceUploadDto
{
    public Guid TrainingId { get; set; }

    public List<Guid> ResourceIds { get; set; } = [];
}