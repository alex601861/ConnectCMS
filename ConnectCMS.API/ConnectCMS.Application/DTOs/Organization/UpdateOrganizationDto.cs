using CMSTrain.Application.Common.Attributes;
using Microsoft.AspNetCore.Http;

namespace CMSTrain.Application.DTOs.Organization;

public class UpdateOrganizationDto : CreateOrganizationDto
{
    public Guid Id { get; set; }
}
