namespace CMSTrain.Client.Models.Responses.TrainingFormat;

public class GetTrainingFormatDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }
}
