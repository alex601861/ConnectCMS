using CMSTrain.Application.DTOs.Class;
using CMSTrain.Application.DTOs.Count;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/class")]
public class ClassController(IClassService classService) : BaseController<ClassController>
{
    [HttpGet("{trainingId:guid}")]
    public IActionResult GetAllClasses(Guid trainingId, int pageNumber, int pageSize, string? search, int? status)
    {
        var classes = classService.GetAllClasses(trainingId, pageNumber, pageSize, out var rowCount, search, status);

        return Ok(new CollectionDto<GetClassDto>(classes, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Classes successfully retrieved."
        });
    }
    
    [HttpGet("list/{trainingId:guid}")]
    public IActionResult GetAllClasses(Guid trainingId)
    {
        var classes = classService.GetAllClasses(trainingId);

        return Ok(new ResponseDto<List<GetClassDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Classes successfully retrieved.",
            Result = classes
        });
    }

    [HttpGet("trainers/{trainingId:guid}")]
    public IActionResult GetAllClassesForTrainers(Guid trainingId, int pageNumber, int pageSize, string? search, int? status)
    {
        var classes = classService.GetAllClassesForTrainers(trainingId, pageNumber, pageSize, out var rowCount, search, status);

        return Ok(new CollectionDto<GetClassForTrainersDto>(classes, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Classes successfully retrieved."
        });
    }

    [HttpGet("trainers/list/{trainingId:guid}")]
    public IActionResult GetAllClassesForTrainers(Guid trainingId)
    {
        var result = classService.GetAllClassesForTrainers(trainingId);

        return Ok(new ResponseDto<List<GetClassForTrainersDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Classes successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("candidates/{trainingId:guid}")]
    public IActionResult GetAllClassesForCandidates(Guid trainingId, int pageNumber, int pageSize, string? search, int? status)
    {
        var classes = classService.GetAllClassesForCandidates(trainingId, pageNumber, pageSize, out var rowCount, search, status);

        return Ok(new CollectionDto<GetClassForCandidatesDto>(classes, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Classes successfully retrieved."
        });
    }
     
    [HttpGet("candidates/list/{trainingId:guid}")]
    public IActionResult GetAllClassesForCandidates(Guid trainingId)
    {
        var result = classService.GetAllClassesForCandidates(trainingId);

        return Ok(new ResponseDto<List<GetClassForCandidatesDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Classes successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("training-candidates/{trainingCandidateId:guid}")]
    public IActionResult GetAllCandidateClasses(Guid trainingCandidateId, int pageNumber, int pageSize)
    {
        var classes = classService.GetAllCandidateClasses(trainingCandidateId, pageNumber, pageNumber, out var rowCount);

        return Ok(new CollectionDto<GetClassForCandidatesDto>(classes, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Classes successfully retrieved."
        });
    }
    
    [HttpGet("training-candidates/list/{trainingCandidateId:guid}")]
    public IActionResult GetAllCandidateClasses(Guid trainingCandidateId)
    {
        var classes = classService.GetAllCandidateClasses(trainingCandidateId);

        return Ok(new ResponseDto<List<GetClassForCandidatesDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Classes successfully retrieved.",
            Result = classes
        });
    }
    
    [HttpGet("candidate/count/{classId:guid}")]
    public IActionResult GetClassDetailsCountForCandidate(Guid classId)
    {
        var result = classService.GetClassDetailsCountForCandidate(classId);

        return Ok(new ResponseDto<ClassCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Class details count successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("client/count/{classId:guid}")]
    public IActionResult GetClassDetailsCountForClient(Guid classId)
    {
        var result = classService.GetClassDetailsCountForClient(classId);

        return Ok(new ResponseDto<ClassCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Class details count successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("trainer/count/{classId:guid}")]
    public IActionResult GetClassDetailsCountForTrainer(Guid classId)
    {
        var result = classService.GetClassDetailsCountForClient(classId);

        return Ok(new ResponseDto<ClassCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Class details count successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("admin/count/{classId:guid}")]
    public IActionResult GetClassDetailsCountForAdmin(Guid classId)
    {
        var result = classService.GetClassDetailsCountForAdmin(classId);

        return Ok(new ResponseDto<ClassCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Class details count successfully retrieved.",
            Result = result
        });
    }

    [HttpGet("details/{classId:guid}")]
    public IActionResult GetClassById(Guid classId)
    {
        var result = classService.GetClassById(classId);

        return Ok(new ResponseDto<GetClassForTrainersDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Class successfully retrieved.",
            Result = result
        });
    }

    [HttpPost]
    public IActionResult InsertClass(CreateClassDto @class)
    {
        classService.InsertClass(@class);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Class successfully inserted.",
            Result = true
        });
    }
    
    [HttpPut]
    public IActionResult UpdateClass(UpdateClassDto @class)
    {
        classService.UpdateClass(@class);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Class successfully updated.",
            Result = true
        });
    }

    [HttpDelete("{classId:guid}")]
    public IActionResult DeleteClass(Guid classId)
    {
        classService.DeleteClass(classId);
        
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The status of class successfully updated.",
            Result = true
        });
    }
}
