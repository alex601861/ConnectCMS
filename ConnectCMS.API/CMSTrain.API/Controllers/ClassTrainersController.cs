using CMSTrain.Application.Common.Response;
using CMSTrain.Application.DTOs.ClassTrainers;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/class-trainers")]
public class ClassTrainersController(IClassTrainersService classTrainersService) : BaseController<ClassTrainersController>
{
    // TODO: Display the logged in trainer in the first index of the resulted collection.
    [HttpGet("training/{trainingId:guid}")]
    public IActionResult GetAllTrainersForTraining(Guid trainingId, int pageNumber, int pageSize, string? search)
    {
        var assignedTrainers = classTrainersService.GetAllTrainersForTraining(trainingId, pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<GetAssignedTrainersDto>(assignedTrainers, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainers successfully retrieved."
        });
    }

    // TODO: Display the logged in trainer in the first index of the resulted collection.
    [HttpGet("training/list/{trainingId:guid}")]
    public IActionResult GetAllTrainersForTraining(Guid trainingId, string? search)
    {
        var result = classTrainersService.GetAllTrainersForTraining(trainingId, search);

        return Ok(new ResponseDto<List<GetAssignedTrainersDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainers successfully retrieved.",
            Result = result
        });
    }
    
    // TODO: Display the logged in trainer in the first index of the resulted collection.
    [HttpGet("class/{classId:guid}")]
    public IActionResult GetAllTrainersForClass(Guid classId, int pageNumber, int pageSize, string? search)
    {
        var assignedTrainers = classTrainersService.GetAllTrainersForClass(classId, pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<GetAssignedTrainersDto>(assignedTrainers, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainers successfully retrieved."
        });
    }

    // TODO: Display the logged in trainer in the first index of the resulted collection.
    [HttpGet("class/list/{classId:guid}")]
    public IActionResult GetAllTrainersForClass(Guid classId, string? search)
    {
        var result = classTrainersService.GetAllTrainersForClass(classId, search);

        return Ok(new ResponseDto<List<GetAssignedTrainersDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainers successfully retrieved.",
            Result = result
        });
    }
    
    [HttpPost("assign")]
    public IActionResult AssignTrainersToClass(AssignTrainersDto assignTrainersDto)
    {
        classTrainersService.AssignTrainersToClass(assignTrainersDto);
       
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainers successfully assigned to the following training.",
            Result = true
        });
    }
    
    [HttpPut("description/{classTrainerId:guid}")]
    public IActionResult UpdateTrainerDescription(Guid classTrainerId, UpdateClassTrainerDescriptionDto classTrainerDescription)
    {
        classTrainersService.UpdateTrainerDescription(classTrainerId, classTrainerDescription);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The description for the respective trainer has been successfully updated.",
            Result = true
        });
    }
    
    [HttpGet("descriptions/training/{trainingId:guid}/{trainerId:guid}")]
    public IActionResult GetTrainerDescriptionsOnTraining(Guid trainingId, Guid trainerId)
    {
        var result = classTrainersService.GetTrainerDescriptionsOnTraining(trainingId, trainerId);

        return Ok(new ResponseDto<GetTrainerDescriptionDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The description for the respective trainer has been successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("descriptions/class/{classId:guid}/{trainerId:guid}")]
    public IActionResult GetTrainerDescriptionsOnClass(Guid classId, Guid trainerId)
    {
        var result = classTrainersService.GetTrainerDescriptionsOnClass(classId, trainerId);

        return Ok(new ResponseDto<GetTrainerDescriptionDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The description for the respective trainer has been successfully retrieved.",
            Result = result
        });
    }
}