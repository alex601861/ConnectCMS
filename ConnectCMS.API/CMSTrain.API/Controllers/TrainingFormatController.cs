using CMSTrain.Application.Common.Response;
using CMSTrain.Application.DTOs.TrainingFormat;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/training-format")]
public class TrainingFormatController(ITrainingFormatService trainingFormatService) : BaseController<TrainingFormatController>
{
    [HttpGet]
    public IActionResult GetAllTrainingFormats(int pageNumber, int pageSize, string? search, bool? isActive)
    {
        var trainingFormats = trainingFormatService.GetAllTrainingFormats(pageNumber, pageSize, out var rowCount, search, isActive);

        return Ok(new CollectionDto<GetTrainingFormatDto>(trainingFormats, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training formats successfully retrieved."
        });
    }

    [HttpGet("list")]
    public IActionResult GetAllTrainingFormats(string? search, bool? isActive)
    {
        var result = trainingFormatService.GetAllTrainingFormats(search, isActive);

        return Ok(new ResponseDto<List<GetTrainingFormatDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training formats successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("{trainingFormatId:guid}")]
    public IActionResult GetTrainingFormatById(Guid trainingFormatId)
    {
        var result = trainingFormatService.GetTrainingFormatById(trainingFormatId);
        
        return Ok(new ResponseDto<GetTrainingFormatDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training format successfully retrieved.",
            Result = result
        });
    }

    [HttpPost]
    public IActionResult InsertTrainingFormat(CreateTrainingFormatDto trainingFormat)
    {
        trainingFormatService.InsertTrainingFormat(trainingFormat);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training format successfully created.",
            Result = true
        });
    }

    [HttpPut]
    public IActionResult UpdateTrainingFormat(UpdateTrainingFormatDto trainingFormat)
    {
        trainingFormatService.UpdateTrainingFormat(trainingFormat);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training format successfully updated.",
            Result = true
        });
    }

    [HttpPatch("{trainingFormatId:guid}")]
    public IActionResult ActivateDeactivateTrainingFormat(Guid trainingFormatId)
    {
        trainingFormatService.ActivateDeactivateTrainingFormat(trainingFormatId);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The status of training format successfully updated.",
            Result = true
        });
    }
}
