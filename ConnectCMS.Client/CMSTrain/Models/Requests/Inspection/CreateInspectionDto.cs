using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Models.Requests.Inspection;

public class CreateInspectionDto
{
    public string Name { get; set; } = "";
    
    public string Description { get; set; } = "";
    
    public InspectionType InspectionType { get; set; } = InspectionType.None;

    public IBrowserFile? Image { get; set; }
}