using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.DTOs.Heading;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/heading")]
public class HeadingController(IHeadingService headingService) : BaseController<HeadingController>
{
    [HttpGet("{headingType}/{facetType}/{inspectionType}")]
    public IActionResult GetAllHeadings(HeadingType headingType, FacetType facetType, InspectionType inspectionType, int pageNumber, int pageSize, bool? isActive, string? search)
    {
        var result = headingService.GetAllHeadings(headingType, facetType, inspectionType, pageNumber, pageSize, out var rowCount, isActive, search);

        return Ok(new CollectionDto<GetHeadingDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Headings successfully retrieved."
        });
    }

    [HttpGet("list/{headingType}/{facetType}/{inspectionType}")]
    public IActionResult GetAllHeadings(HeadingType headingType, FacetType facetType, InspectionType inspectionType, bool? isActive, string? search)
    {
        var result = headingService.GetAllHeadings(headingType, facetType, inspectionType, isActive, search);

        return Ok(new ResponseDto<List<GetHeadingDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Headings successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("parent/heading/{facetType}/{inspectionType}")]
    public IActionResult GetAllParentHeadings(FacetType facetType, InspectionType inspectionType)
    {
        var result = headingService.GetAllParentHeadings(facetType, inspectionType);

        return Ok(new ResponseDto<List<GetHeadingModuleDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Parent headings successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("sub/heading")]
    public IActionResult GetAllSubHeadings()
    {
        var result = headingService.GetAllSubHeadings();

        return Ok(new ResponseDto<List<GetHeadingModuleDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Sub-headings successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("{headingId:guid}")]
    public IActionResult GetHeadingById(Guid headingId)
    {
        var result = headingService.GetHeadingById(headingId);

        return Ok(new ResponseDto<GetHeadingDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Heading successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("count/{facetType}/{inspectionType}")]
    public IActionResult GetAllHeadingCount(FacetType facetType, InspectionType inspectionType)
    {
        var result = headingService.GetHeadingCount(facetType, inspectionType);

        return Ok(new ResponseDto<GetHeadingCountDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Heading count successfully retrieved.",
            Result = result
        });
    }
    
    [HttpPost]
    public IActionResult InsertHeading(CreateHeadingDto heading)
    {
        headingService.InsertHeading(heading);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Heading successfully inserted.",
            Result = true
        });
    }

    [HttpPut]
    public IActionResult UpdateHeading(UpdateHeadingDto heading)
    {
        headingService.UpdateHeading(heading);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Heading successfully updated.",
            Result = true
        });
    }

    [HttpPatch("{headingId:guid}")]
    public IActionResult ActivateDeactivateHeading(Guid headingId)
    {
        headingService.ActivateDeactivateHeading(headingId);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Heading activation status successfully toggled.",
            Result = true
        });
    }

    [HttpDelete("{headingId:guid}")]
    public IActionResult DeleteHeading(Guid headingId)
    {
        headingService.DeleteHeading(headingId);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Heading successfully deleted.",
            Result = true
        });
    }
}