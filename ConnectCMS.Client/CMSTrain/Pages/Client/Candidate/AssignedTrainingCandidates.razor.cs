using CMSTrain.Client.Models.Responses.ClientOrganization;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Pages.Client.Candidate;

public partial class AssignedTrainingCandidates : ComponentBase
{
    [Parameter] public List<GetClientOrganizationUsersDto> ClientOrganizationUsers { get; set; } = [];
}