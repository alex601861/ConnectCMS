using Microsoft.AspNetCore.Http;

namespace CMSTrain.Application.DTOs.Training;

public class CreateTrainingDto
{
    public string Title { get; set; }

    public string Description { get; set; }

    public decimal Longitude { get; set; }

    public decimal Latitude { get; set; }
    
    public Guid TrainingFormatId { get; set; }

    public string LocationDetails { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IFormFile? Image { get; set; }
}
