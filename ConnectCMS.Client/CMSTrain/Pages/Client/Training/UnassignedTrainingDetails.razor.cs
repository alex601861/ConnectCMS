using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.Client.Training;

public partial class UnassignedTrainingDetails
{
    [Parameter] public Guid TrainingId { get; set; }
}