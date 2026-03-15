using CMSTrain.Client.Models.Responses.Training;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.State.Training;

public partial class AssignedTrainerDetails
{
    [Parameter] public GetAssignedTrainingsTrainersDto AssignedTrainers { get; set; } = new();
}