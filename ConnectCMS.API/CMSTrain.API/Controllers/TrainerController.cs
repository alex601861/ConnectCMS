using CMSTrain.Application.Common.Response;
using CMSTrain.Application.DTOs.ClassTrainers;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/trainers")]
public class TrainerController(IClassTrainersService classTrainersService) : BaseController<TrainerController>
{
    [HttpGet]
    public IActionResult GetAllActiveTrainers(int pageNumber, int pageSize)
    {
        var trainers = classTrainersService.GetAllActiveTrainers(pageNumber, pageSize, out var rowCount);

        return Ok(new CollectionDto<GetTrainersDto>(trainers, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainers successfully retrieved."
        });
    }

    [HttpGet("list")]
    public IActionResult GetAllActiveTrainers()
    {
        var trainers = classTrainersService.GetAllActiveTrainers();

        return Ok(new ResponseDto<List<GetTrainersDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainers successfully retrieved.",
            Result = trainers
        });
    }
}
