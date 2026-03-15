using CMSTrain.Client.Models.Responses.Training;

namespace CMSTrain.Client.Models.Responses.ClassTrainers;

public class GetAssignedTrainingsDto : GetTrainingDto
{
    public int AssignedClasses { get; set; }
    
    public string NextClassDate { get; set; }
}