using CMSTrain.Application.Common.Response;
using CMSTrain.Application.DTOs.Organization;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/organization")]
public class OrganizationController(IOrganizationService organizationService) : BaseController<OrganizationController>
{
    [HttpGet]
    public IActionResult GetAllOrganizations(int pageNumber, int pageSize, string? search, bool? isActive)
    {
        var organizations = organizationService.GetAllOrganizations(pageNumber, pageSize, out var rowCount, search, isActive);

        return Ok(new CollectionDto<GetOrganizationDto>(organizations, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Organization successfully retrieved."
        });
    }

    [HttpGet("list")]
    public IActionResult GetAllOrganizations(string? search, bool? isActive)
    {
        var result = organizationService.GetAllOrganizations(search, isActive);

        return Ok(new ResponseDto<List<GetOrganizationDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Organization successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("{organizationId:guid}")]
    public IActionResult GetOrganizationById(Guid organizationId)
    {
        var result = organizationService.GetOrganizationById(organizationId);

        return Ok(new ResponseDto<GetOrganizationDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Organization of provided identifier successfully fetched.",
            Result = result
        });
    }

    [HttpPost]
    public IActionResult InsertOrganization([FromForm] CreateOrganizationDto organization)
    {
        organizationService.InsertOrganization(organization);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Organization successfully created.",
            Result = true
        });
    }

    [HttpPut]
    public IActionResult UpdateOrganization([FromForm] UpdateOrganizationDto organization)
    {
        organizationService.UpdateOrganization(organization);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Organization successfully updated.",
            Result = true
        });
    }

    [HttpPatch("{organizationId:guid}")]
    public IActionResult ActivateDeactivateOrganization(Guid organizationId)
    {
        organizationService.ActivateDeactivateOrganization(organizationId);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The status of organization successfully updated.",
            Result = true
        });
    }
    
    [HttpDelete("{organizationId:guid}")]
    public IActionResult DeleteOrganization(Guid organizationId)
    {
        organizationService.DeleteOrganization(organizationId);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The organization has been permanently deleted.",
            Result = true
        });
    }
}
