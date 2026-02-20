using CMSTrain.Application.DTOs.Count;
using CMSTrain.Application.DTOs.Training;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.DTOs.ClassTrainers;
using CMSTrain.Application.DTOs.Organization;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.TrainingCandidate;

namespace CMSTrain.Controllers;

[Route("api/training")]
public class TrainingController(ITrainingService trainingService, 
    ITrainingCandidateService trainingCandidateService,
    IClassTrainersService classTrainersService) : BaseController<TrainingController>
{
    #region Admin Trainings
    [HttpGet("{statusAction:int}")]
    public IActionResult GetAllTrainings(int statusAction, int pageNumber, int pageSize, string? search, bool? isActive)
    {
        var result = trainingService.GetAllTrainings(statusAction, pageNumber, pageSize, out var rowCount, search, isActive);

        return Ok(new CollectionDto<GetTrainingDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved."
        });
    }

    [HttpGet("list/{statusAction:int}")]
    public IActionResult GetAllTrainings(int statusAction, string? search, bool? isActive)
    {
        var result = trainingService.GetAllTrainings(statusAction, search, isActive);

        return Ok(new ResponseDto<List<GetTrainingDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("count")]
    public IActionResult GetAvailableTrainingsCount()
    {
        var result = trainingService.GetAvailableTrainingsCount();
        
        return Ok(new ResponseDto<AssignedTrainingCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available trainings count successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("details/{trainingId:guid}")]
    public IActionResult GetTrainingById(Guid trainingId)
    {
        var result = trainingService.GetTrainingById(trainingId);

        return Ok(new ResponseDto<GetTrainingDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("module/count")]
    public IActionResult GetTrainingModuleCount(bool? isActive)
    {
        var result = trainingService.GetTrainingModuleCount(isActive);

        return Ok(new ResponseDto<TrainingModuleCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training models and format count successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("details-count/{trainingId:guid}")]
    public IActionResult GetTrainingDetailsCount(Guid trainingId)
    {
        var result = trainingService.GetTrainingDetailsCount(trainingId);

        return Ok(new ResponseDto<TrainingDetailsCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training details count successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("client-organizations/{trainingId:guid}")]
    public IActionResult GetAllAssignedClientOrganizations(Guid trainingId)
    {
        var result = trainingService.GetAllAssignedClientOrganizations(trainingId);

        return Ok(new ResponseDto<List<GetOrganizationDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned client organizations successfully retrieved.",
            Result = result
        });
    }
    
    [HttpPost]
    public IActionResult InsertTraining([FromForm] CreateTrainingDto training)
    {
        trainingService.InsertTraining(training);
        
        return Ok(new ResponseDto<bool> 
        { 
            StatusCode = (int)HttpStatusCode.OK, 
            Message = "Training successfully created.",
            Result = true
        });
    }

    [HttpPut]
    public IActionResult UpdateTraining([FromForm] UpdateTrainingDto training)
    {
        trainingService.UpdateTraining(training);
        
        return Ok(new ResponseDto<bool>
        { 
            StatusCode = (int)HttpStatusCode.OK, 
            Message = "Training successfully updated.", 
            Result = true
        });
    }

    [HttpPatch("{trainingId:guid}")]
    public IActionResult ActivateDeactivateTraining(Guid trainingId)
    {
        trainingService.ActivateDeactivateTraining(trainingId);

        return Ok(new ResponseDto<bool>() 
        { 
            StatusCode = (int)HttpStatusCode.OK, 
            Message = "The status of training successfully updated.", 
            Result = true
        });
    }
    #endregion

    #region Trainer Trainings
    [HttpGet("available/trainer")]
    public IActionResult GetAllTrainingsForTrainer(int pageNumber, int pageSize, string? search = null)
    {
        var result = classTrainersService.GetAllAvailableTrainingsForTrainers(pageNumber, pageSize, out var rowCount, search);
        
        return Ok(new CollectionDto<GetTrainingDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
        });
    }
    
    [HttpGet("available/trainer/list")]
    public IActionResult GetAllTrainingsForTrainer(string? search = null)
    {
        var result = classTrainersService.GetAllAvailableTrainingsForTrainers(search);
        
        return Ok(new ResponseDto<List<GetTrainingDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("available/trainer/count")]
    public IActionResult GetAllAvailableTrainingCountForTrainers()
    {
        var result = classTrainersService.GetAllAvailableTrainingCountForTrainers();
        
        return Ok(new ResponseDto<AvailableTrainingCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available trainings count successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("assigned/trainer/{statusAction:int}")]
    public IActionResult GetAllAssignedTrainingsForTrainers(int statusAction, int pageNumber, int pageSize, string? search = null)
    {
        var result = classTrainersService.GetAllAssignedTrainingsForTrainers(statusAction, pageNumber, pageSize, out var rowCount, search);
        
        return Ok(new CollectionDto<GetAssignedTrainingsDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
        });
    }
    
    [HttpGet("assigned/trainer/list/{statusAction:int}")]
    public IActionResult GetAllAssignedTrainingsForTrainers(int statusAction, string? search = null)
    {
        var result = classTrainersService.GetAllAssignedTrainingsForTrainers(statusAction, search);
        
        return Ok(new ResponseDto<List<GetAssignedTrainingsDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("assigned/trainer/count")]
    public IActionResult GetAllAssignedTrainingCountForTrainers()
    {
        var result = classTrainersService.GetAllAssignedTrainingCountForTrainers();
        
        return Ok(new ResponseDto<AssignedTrainingCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned trainings count successfully retrieved.",
            Result = result
        });
    }
    #endregion
    
    #region Candidate Trainings
    [HttpGet("available/candidate/{request:int}")]
    public IActionResult GetAllTrainingsForCandidate(int request, int pageNumber, int pageSize, string? search = null)
    {
        var result = trainingCandidateService.GetAllTrainingsForCandidate(request, pageNumber, pageSize, out var rowCount, search);
        
        return Ok(new CollectionDto<GetAllTrainingsForCandidate>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
        });
    }
    
    [HttpGet("available/candidate/list/{request:int}")]
    public IActionResult GetAllTrainingsForCandidate(int request, string? search = null)
    {
        var result = trainingCandidateService.GetAllTrainingsForCandidate(request, search);
        
        return Ok(new ResponseDto<List<GetAllTrainingsForCandidate>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("available/candidate/count")]
    public IActionResult GetAvailableTrainingCountsForCandidate()
    {
        var result = trainingCandidateService.GetAllAvailableTrainingCountsForCandidate();
        
        return Ok(new ResponseDto<AvailableTrainingCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available trainings count successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("assigned/candidate/{statusAction:int}")]
    public IActionResult GetAllAssignedTrainingsForCandidate(int statusAction, int pageNumber, int pageSize, string? search = null)
    {
        var result = trainingCandidateService.GetAllAssignedTrainingsForCandidate(statusAction, pageNumber, pageSize, out var rowCount, search);
        
        return Ok(new CollectionDto<GetAllTrainingsForCandidate>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
        });
    }
    
    [HttpGet("assigned/candidate/list/{statusAction:int}")]
    public IActionResult GetAllAssignedTrainingsForCandidate(int statusAction, string? search = null)
    {
        var result = trainingCandidateService.GetAllAssignedTrainingsForCandidate(statusAction, search);
        
        return Ok(new ResponseDto<List<GetAllTrainingsForCandidate>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("assigned/candidate/count")]
    public IActionResult GetAllAssignedTrainingCountsForCandidate()
    {
        var result = trainingCandidateService.GetAllAssignedTrainingCountsForCandidate();
        
        return Ok(new ResponseDto<AssignedTrainingCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available trainings count successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("assigned/candidate/training/count/{trainingId:guid}")]
    public IActionResult GetTrainingDetailsCountForCandidate(Guid trainingId)
    {
        var result = trainingCandidateService.GetTrainingDetailsCountForCandidate(trainingId);
        
        return Ok(new ResponseDto<TrainingDetailsCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available trainings count successfully retrieved.",
            Result = result
        });
    }
    #endregion
    
    #region Client Trainings
    [HttpGet("available/client/{request:int}")]
    public IActionResult GetAllTrainingsForClient(int request, int pageNumber, int pageSize, string? search = null)
    {
        var result = trainingCandidateService.GetAllTrainingsForClient(request, pageNumber, pageSize, out var rowCount, search);
        
        return Ok(new CollectionDto<GetAllTrainingsForClient>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
        });
    }
    
    [HttpGet("available/client/list/{request:int}")]
    public IActionResult GetAllTrainingsForClient(int request, string? search = null)
    {
        var result = trainingCandidateService.GetAllTrainingsForClient(request, search);
        
        return Ok(new ResponseDto<List<GetAllTrainingsForClient>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("available/client/count")]
    public IActionResult GetAllAvailableTrainingCountsForClient()
    {
        var result = trainingCandidateService.GetAllAvailableTrainingCountsForClient();
        
        return Ok(new ResponseDto<AvailableTrainingCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("assigned/client/{statusAction:int}")]
    public IActionResult GetAllAssignedTrainingsForClient(int statusAction, int pageNumber, int pageSize, string? search = null)
    {
        var result = trainingCandidateService.GetAllAssignedTrainingsForClient(statusAction, pageNumber, pageSize, out var rowCount, search);
        
        return Ok(new CollectionDto<GetAllTrainingsForClient>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
        });
    }
    
    [HttpGet("assigned/client/list/{statusAction:int}")]
    public IActionResult GetAllAssignedTrainingsForClient(int statusAction, string? search = null)
    {
        var result = trainingCandidateService.GetAllAssignedTrainingsForClient(statusAction, search);
        
        return Ok(new ResponseDto<List<GetAllTrainingsForClient>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("assigned/client/count")]
    public IActionResult GetAllAssignedTrainingCountsForClient()
    {
        var result = trainingCandidateService.GetAllAssignedTrainingCountsForClient();
        
        return Ok(new ResponseDto<AssignedTrainingCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Trainings successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("assigned/client/training/count/{trainingId:guid}")]
    public IActionResult GetTrainingDetailsCountForClient(Guid trainingId)
    {
        var result = trainingCandidateService.GetTrainingDetailsCountForClient(trainingId);
        
        return Ok(new ResponseDto<TrainingDetailsCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Available trainings count successfully retrieved.",
            Result = result
        });
    }
    #endregion

    #region Generic Trainings
    [HttpGet("details/inspection/{trainingInspectionId:guid}")]
    public IActionResult GetTrainingDetailsByInspection(Guid trainingInspectionId)
    {
        var result = trainingService.GetTrainingDetailsByInspection(trainingInspectionId);

        return Ok(new ResponseDto<GetTrainingDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("details/questionnaire/{questionnaireId:guid}")]
    public IActionResult GetTrainingDetailsByQuestionnaire(Guid questionnaireId)
    {
        var result = trainingService.GetTrainingDetailsByQuestionnaire(questionnaireId);

        return Ok(new ResponseDto<GetTrainingDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Training successfully retrieved.",
            Result = result
        });
    }
    #endregion
}
