using CMSTrain.Application.DTOs.Training;

namespace CMSTrain.Application.DTOs.ClassTrainers;

public class GetAssignedTrainingsDto : GetTrainingDto
{
    public int AssignedClasses { get; set; }
    
    public string NextClassDate { get; set; }
    
    public string Description { get; set; }
}