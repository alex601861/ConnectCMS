using CMSTrain.Application.DTOs.Training;

namespace CMSTrain.Application.DTOs.Dashboard;

public class GetActiveTrainings
{
    public Guid Id { get; set; }
    
    public string Title { get; set; }   
    
    public string? ImageUrl { get; set; }
    
    public string Description { get; set; } 
    
    public string LocationDetails { get; set; }
    
    public decimal Longitude { get; set; }
    
    public decimal Latitude { get; set; }
    
    public Guid TrainingFormatId { get; set; }
    
    public string TrainingFormatName { get; set; }
    
    public string Date { get; set; }
    
    public GetAssignedTrainingsTrainersDto AssignedTrainer { get; set; }
}