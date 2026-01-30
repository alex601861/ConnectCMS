using CMSTrain.Application.DTOs.Class;

namespace CMSTrain.Application.DTOs.ClassTrainers;

public class GetTrainerDescriptionDto
{
    public GetTrainersDto Trainer { get; set; }
    
    public List<GetClassTrainerDescriptionDto> Classes { get; set; } = [];
}

public class GetClassTrainerDescriptionDto : GetClassDto
{
    public string Description { get; set; }
}