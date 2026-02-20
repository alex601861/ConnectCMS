using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.DTOs.Strategy;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/strategy-trait")]
public class StrategyTraitController(IStrategyTraitService strategyTraitService) : BaseController<StrategyTraitController>
{
    [HttpGet("{traitType}")]
    public IActionResult GetAllStrategies(StrategicType traitType, int pageNumber, int pageSize, string? search)
    {
        var result = strategyTraitService.GetAllStrategies(traitType, pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<GetStrategyDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategies successfully retrieved."
        });
    }

    [HttpGet("list")]
    public IActionResult GetAllStrategies()
    {
        var result = strategyTraitService.GetAllStrategies();

        return Ok(new ResponseDto<List<GetStrategyDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategies retrieved successfully.",
            Result = result
        });
    }
    
    [HttpGet("modules/{type}")]
    public IActionResult GetAllStrategyModules(StrategicType type)
    {
        var result = strategyTraitService.GetAllStrategyModules(type);

        return Ok(new ResponseDto<List<GetStrategyModuleDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategies retrieved successfully.",
            Result = result
        });
    }
    
    [HttpGet("modules/traits")]
    public IActionResult GetAllStrategyTraitResults(string? strengthIds, string? weaknessIds)
    {
        var result = strategyTraitService.GetAllStrategyTraitResults(strengthIds, weaknessIds);

        return Ok(new ResponseDto<GetAllStrategyTraitResultsDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategies retrieved successfully.",
            Result = result
        });
    }
    
    [HttpGet("{strategyId:guid}")]
    public IActionResult GetStrategyById(Guid strategyId)
    {
        var strategy = strategyTraitService.GetStrategyById(strategyId);

        return Ok(new ResponseDto<GetStrategyDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategy successfully retrieved.",
            Result = strategy
        });
    }

    [HttpGet("details")]
    public IActionResult GetStrategyDetails()
    {
        var strategyDetails = strategyTraitService.GetStrategyDetails();

        return Ok(new ResponseDto<GetStrategyDetailsDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategy details successfully retrieved.",
            Result = strategyDetails
        });
    }

    [HttpPost]
    public IActionResult InsertStrategy(InsertStrategyDto strategy)
    {
        strategyTraitService.InsertStrategy(strategy);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategy successfully inserted.",
            Result = true
        });
    }

    [HttpPut]
    public IActionResult UpdateStrategy(UpdateStrategyDto strategy)
    {
        strategyTraitService.UpdateStrategy(strategy);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategy successfully updated.",
            Result = true
        });
    }

    [HttpDelete("{strategyId:guid}")]
    public IActionResult DeleteStrategy(Guid strategyId)
    {
        strategyTraitService.DeleteStrategy(strategyId);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategy successfully deleted.",
            Result = true
        });
    }

    [HttpPost("upload-strategy-details")]
    public IActionResult UploadStrategyDetails(UploadStrategyDetailsDto strategyDetails)
    {
        strategyTraitService.UploadStrategyDetails(strategyDetails);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategy details successfully uploaded.",
            Result = true
        });
    }
    
    [HttpPost("upload-strategy-traits")]
    public IActionResult UploadStrategyTraitQuestionnaire(UploadStrategyTraitQuestionnaireDto strategyDetails)
    {
        strategyTraitService.UploadStrategyTraitQuestionnaire(strategyDetails);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Your responses have been successfully saved, thank you :)",
            Result = true
        });
    }
    
    [HttpGet("count")]
    public IActionResult GetStrategicTraitCount()
    {
        var result = strategyTraitService.GetStrategicTraitCount();

        return Ok(new ResponseDto<GetStrategicTraitCountDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategy details successfully fetched.",
            Result = result
        });
    }
    
    [HttpGet("responses")]
    public IActionResult GetStrategyTraitQuestionnaireResponses(int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate)
    {
        var result = strategyTraitService.GetStrategyTraitQuestionnaireResponses(pageNumber, pageSize, out var rowCount, startDate, endDate);

        return Ok(new CollectionDto<GetStrategyTraitQuestionnaireDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategy details successfully uploaded."
        });
    }
    
    [HttpGet("responses/list")]
    public IActionResult GetStrategyTraitQuestionnaireResponses()
    {
        var result = strategyTraitService.GetStrategyTraitQuestionnaireResponses();

        return Ok(new ResponseDto<List<GetStrategyTraitQuestionnaireDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategy details successfully uploaded.",
            Result = result
        });
    }
    
    [HttpGet("responses/{userId:guid}")]
    public IActionResult GetStrategyTraitQuestionnaireResponsesByUserId(Guid userId, int pageNumber, int pageSize)
    {
        var result = strategyTraitService.GetStrategyTraitQuestionnaireResponsesByUserId(userId, pageNumber, pageSize, out var rowCount);

        return Ok(new CollectionDto<GetStrategyTraitQuestionnaireDto>(result, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategy details successfully uploaded."
        });
    }
    
    [HttpGet("responses/list/{userId:guid}")]
    public IActionResult GetStrategyTraitQuestionnaireResponsesByUserId(Guid userId)
    {
        var result = strategyTraitService.GetStrategyTraitQuestionnaireResponsesByUserId(userId);

        return Ok(new ResponseDto<List<GetStrategyTraitQuestionnaireDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategy details successfully uploaded.",
            Result = result
        });
    }
    
    [HttpGet("responses/details/{responseId:guid}")]
    public IActionResult GetStrategyTraitQuestionnaireDetails(Guid responseId)
    {
        var result = strategyTraitService.GetStrategyTraitQuestionnaireDetails(responseId);

        return Ok(new ResponseDto<GetStrategyTraitQuestionnaireDetailsDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Strategy details successfully uploaded.",
            Result = result
        });
    }
}