namespace CMSTrain.Application.DTOs.ClassTrainers;

public class AssignTrainersDto
{
    public Guid ClassTrainerId { get; set; }

    public Guid ClassId { get; set; }

    public List<Guid> TrainerIds { get; set; }
}
