namespace CMSTrain.Application.DTOs.ClassTrainers;

public class UpdateClassTrainerDescriptionDto
{
    public Guid ClassTrainerId { get; set; }
    
    public string Description { get; set; }
}