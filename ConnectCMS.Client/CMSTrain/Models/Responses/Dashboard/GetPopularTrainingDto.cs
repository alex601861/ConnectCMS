using CMSTrain.Client.Models.Responses.Training;

namespace CMSTrain.Client.Models.Responses.Dashboard;

public class GetPopularTrainingDto
{
    public Guid Id { get; set; }    

    public string Title { get; set; }

    public string Description { get; set; }
    
    public string Date { get; set; }
    
    public string Location { get; set; }

    public decimal Longitude { get; set; }
    
    public decimal Latitude { get; set; }
    
    public Guid TrainingFormatId { get; set; }
    
    public string TrainingFormat { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public int AcceptedRequests { get; set; }

    public GetAssignedTrainingsTrainersDto AssignedTrainers { get; set; } = new();
}