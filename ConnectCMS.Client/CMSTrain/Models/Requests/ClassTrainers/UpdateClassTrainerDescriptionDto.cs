namespace CMSTrain.Client.Models.Requests.ClassTrainers;

public class UpdateClassTrainerDescriptionDto
{
    public Guid ClassTrainerId { get; set; }
    
    public string Description { get; set; }
}