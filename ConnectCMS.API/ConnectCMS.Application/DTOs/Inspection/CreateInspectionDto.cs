using Microsoft.AspNetCore.Http;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Application.DTOs.Inspection;

public class CreateInspectionDto
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public InspectionType InspectionType { get; set; }
    
    public IFormFile? Image { get; set; }
}