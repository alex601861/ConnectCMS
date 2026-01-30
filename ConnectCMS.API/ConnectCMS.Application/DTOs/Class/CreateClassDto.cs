using Microsoft.AspNetCore.Http;

namespace CMSTrain.Application.DTOs.Class;

public class CreateClassDto
{
    public string Title { get; set; }
    
    public Guid TrainingId { get; set; }

    public DateTime Date { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }
    
    public IFormFile? Image { get; set; }
}
