using CMSTrain.Client.Models.Responses.Resource;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.State.Resource;

public partial class ResourcePostDetails
{
    [Parameter] public GetResourceDetailsDto Resource { get; set; } = new();
}