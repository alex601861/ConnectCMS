namespace CMSTrain.Application.DTOs.Dashboard;

public class GetClassDetails
{
    public string Title { get; set; }
    
    public string Location { get; set; }
    
    public string ClassDate { get; set; } 
    
    public string ClassDay { get; set; }    
    
    public string StartTime { get; set; } 
    
    public string EndTime { get; set; }

    public bool IsActive { get; set; }
    
    public Guid TrainingId { get; set; }    
    
    public string TrainingName { get; set; }
}