using CMSTrain.Application.Common.Attributes;
using Microsoft.AspNetCore.Http;
using CMSTrain.Application.DTOs.Identity;

namespace CMSTrain.Application.DTOs.Organization;

public class CreateOrganizationDto
{
    public string Name { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    [FileExamination(5 * 1024 * 1024, true)]
    public IFormFile? ImageUrl { get; set; }
}