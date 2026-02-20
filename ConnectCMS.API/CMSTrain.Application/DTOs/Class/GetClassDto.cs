namespace CMSTrain.Application.DTOs.Class;

public class GetClassDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }
    
    public Guid TrainingId { get; set; }

    public string Training { get; set; }

    public string? ImageUrl { get; set; }
    
    public string Date { get; set; }

    public string StartTime { get; set; }

    public string EndTime { get; set; }
    
    public int AssignedTrainers { get; set; }
    
    public string Status { get; set; }
}