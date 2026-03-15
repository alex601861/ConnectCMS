using CMSTrain.Client.Models.Responses.Class;

namespace CMSTrain.Client.Models.Responses.ClassTrainers;

public class GetTrainerDescriptionDto
{
    public GetTrainersDto Trainer { get; set; } = new();
    
    public List<GetClassTrainerDescriptionDto> Classes { get; set; } = [];
}

public class GetClassTrainerDescriptionDto : GetClassDto
{
    public string Description { get; set; } = string.Empty;
}