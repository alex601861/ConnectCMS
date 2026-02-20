using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.ClientOrganization;

namespace CMSTrain.Controllers;

[Route("api/client-organization")]
public class ClientOrganizationController(IClientOrganizationService clientOrganizationService) : BaseController<ClientOrganizationController>
{
    [HttpGet]
    public IActionResult GetAllClientOrganizations(int pageNumber, int pageSize, string? search, bool? isActive)
    {
        var organizations = clientOrganizationService.GetAllClientOrganizations(pageNumber, pageSize, out var rowCount, search, isActive);

        return Ok(new CollectionDto<GetClientOrganizationDto>(organizations, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Organizations successfully retrieved.",
            Result = organizations
        });
    }
    
    [HttpGet("list")]
    public IActionResult GetAllClientOrganizations(string? search, bool? isActive)
    {
        var organizations = clientOrganizationService.GetAllClientOrganizations(search, isActive);

        return Ok(new ResponseDto<List<GetClientOrganizationDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Organizations successfully retrieved.",
            Result = organizations
        });
    }
    
    [HttpGet("admin")]
    public IActionResult GetAllClientOrganizationsWithoutAdmin()
    {
        var organizations = clientOrganizationService.GetAllClientOrganizationsWithoutAdmin();

        return Ok(new ResponseDto<List<GetClientOrganizationDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Organizations successfully retrieved.",
            Result = organizations
        });
    }
    
    [HttpPost]
    public IActionResult RegisterClientOrganizationAdmin([FromForm] RegisterClientAdminDto clientAdmin)
    {
        clientOrganizationService.RegisterClientOrganizationAdmin(clientAdmin);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Client organization admin successfully registered.",
            Result = true
        });
    }
}