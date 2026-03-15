namespace CMSTrain.Client.Models.Responses.Training;

public class GetTrainingDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string LocationDetails { get; set; }

    public decimal Longitude { get; set; }
    
    public decimal Latitude { get; set; }
    
    public Guid TrainingFormatId { get; set; }

    public string TrainingFormat { get; set; }

    public string StartDate { get; set; }

    public string EndDate { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; }

    public GetAssignedTrainingsTrainersDto AssignedTrainers { get; set; } = new();
}
