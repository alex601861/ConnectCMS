namespace CMSTrain.Client.Models.Requests.ClassTrainers;

public class AssignTrainersDto
{
    public Guid ClassId { get; set; }

    public List<Guid> TrainerIds { get; set; } = [];
}
