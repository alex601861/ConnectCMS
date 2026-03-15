using CMSTrain.Client.Models.Responses.ClassTrainers;

namespace CMSTrain.Client.Models.Responses.Training;

public class GetAssignedTrainingsTrainersDto
{
    public List<GetTrainersDto> Trainers { get; set; } = [];

    public string ClassCount { get; set; } = "";
}