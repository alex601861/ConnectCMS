namespace CMSTrain.Application.DTOs.ClassTrainers;

public class GetAssignedTrainersDto : GetTrainersDto
{
    public Guid ClassTrainerId { get; set; }
    
    public string AssignedDate { get; set; }

    public string AssignedBy { get; set; }

    public string Description { get; set; }
}
