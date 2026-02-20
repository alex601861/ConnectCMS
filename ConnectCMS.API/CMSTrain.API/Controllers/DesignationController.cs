using CMSTrain.Application.Common.Response;
using CMSTrain.Application.DTOs.Designation;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/designation")]
public class DesignationController(IDesignationService designationService) : BaseController<DesignationController>
{
    [HttpGet]
    public IActionResult GetAllDesignations(int pageNumber, int pageSize, string? search, bool? isActive)
    {
        var designations = designationService.GetAllDesignations(pageNumber, pageSize, out var rowCount, search, isActive);

        return Ok(new CollectionDto<GetDesignationDto>(designations, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Designations successfully retrieved.",
            Result = designations
        });
    }
    
    [HttpGet("list")]
    public IActionResult GetAllDesignations(string? search, bool? isActive)
    {
        var result = designationService.GetAllDesignations(search, isActive);

        return Ok(new ResponseDto<List<GetDesignationDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Designations successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("{designationId:guid}")]
    public IActionResult GetDesignationById(Guid designationId)
    {
        var result = designationService.GetDesignationById(designationId);

        return Ok(new ResponseDto<GetDesignationDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Designation of provided identifier successfully fetched.",
            Result = result
        });
    }
    
    [HttpPost]
    public IActionResult InsertDesignation(CreateDesignationDto designation)
    {
        designationService.InsertDesignation(designation);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Designation successfully created.",
            Result = true
        });
    }
    
    [HttpPut]
    public IActionResult UpdateDesignation(UpdateDesignationDto designation)
    {
        designationService.UpdateDesignation(designation);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Designation successfully updated.",
            Result = true
        });
    }
    
    [HttpPatch("{designationId:guid}")]
    public IActionResult ActivateDeactivateDesignation(Guid designationId)
    {
        designationService.ActivateDeactivateDesignation(designationId);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The status of designation successfully updated.",
            Result = true
        });
    }
}