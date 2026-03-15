using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Models.Requests.Class;

public class CreateClassDto
{
    public string Title { get; set; }

    public Guid TrainingId { get; set; }

    public DateTime? Date { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }
    
    public IBrowserFile? Image { get; set; }
}
