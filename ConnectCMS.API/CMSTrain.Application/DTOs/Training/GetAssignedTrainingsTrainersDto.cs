using CMSTrain.Application.DTOs.ClassTrainers;

namespace CMSTrain.Application.DTOs.Training;

public class GetAssignedTrainingsTrainersDto
{
    public List<GetTrainersDto> Trainers { get; set; } = [];

    public string ClassCount { get; set; } = "";
}