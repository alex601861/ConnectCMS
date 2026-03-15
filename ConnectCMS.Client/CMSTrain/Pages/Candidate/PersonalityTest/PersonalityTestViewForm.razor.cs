using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Responses.PersonalityTest;

namespace CMSTrain.Client.Pages.Candidate.PersonalityTest;

public partial class PersonalityTestViewForm
{
    [Parameter] public string Trait { get; set; } = string.Empty;

    [Parameter] public List<PersonalityTestResponseFacet> Facets { get; set; } = [];
}