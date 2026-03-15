using CMSTrain.Client.Models.Responses.Training;

namespace CMSTrain.Client.Models.Responses.Dashboard;

public class GetActiveTrainings
{
    public Guid Id { get; set; }
    
    public string Title { get; set; }   
    
    public string? ImageUrl { get; set; }
    
    public string Description { get; set; } 
    
    public string LocationDetails { get; set; }
    
    public Guid TrainingFormatId { get; set; }
    
    public string TrainingFormatName { get; set; }
    
    public string Date { get; set; }
    
    public GetAssignedTrainingsTrainersDto AssignedTrainers { get; set; } = new();
}