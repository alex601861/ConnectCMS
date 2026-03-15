using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.Trainer.Training;

public partial class UnassignedTrainingDetails
{
    [Parameter] public Guid TrainingId { get; set; }
}