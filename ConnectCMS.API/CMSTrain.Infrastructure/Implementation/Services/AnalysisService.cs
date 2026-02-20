using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Property;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.DTOs.Analysis;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class AnalysisService(IGenericRepository genericRepository, IKeyValuePropertyService keyValuePropertyService) : IAnalysisService
{
    public void UploadUserResponseAnalysis(UploadUserResponseAnalysisDto userResponseAnalysis)
    {
        var userResponse = genericRepository.GetById<UserResponse>(userResponseAnalysis.UserResponseId)
                           ?? throw new NotFoundException("The following user response has not been saved.");

        var userResponseAnalysisModel = new UserResponseAnalysis()
        {
            UserResponseId = userResponse.Id,
            Title = userResponseAnalysis.Title,
            Description = new KeyValueProperty()
            {
                Key = "Description",
                Value = userResponseAnalysis.Description
            },
            Scores = new KeyValueProperty()
            {
                Key = "Scores",
                Value = userResponseAnalysis.Score
            }
        };

        genericRepository.Insert(userResponseAnalysisModel);
    }

    public GetUserResponseAnalysisDto GetUserResponseAnalysisDetailsForFeedbacks(Guid userResponseId)
    {
        var userResponse = genericRepository.GetById<UserResponse>(userResponseId)
                           ?? throw new NotFoundException("The following user response has not been saved.");

        var userResponseAnalysis =
            genericRepository.GetFirstOrDefault<UserResponseAnalysis>(x => x.UserResponseId == userResponse.Id);

        if (userResponseAnalysis == null) return new GetUserResponseAnalysisDto();
        
        var userResponseAnalysisModel = new GetUserResponseAnalysisDto()
        {
            Id = userResponseAnalysis.Id,
            UserResponseId = userResponseAnalysis.UserResponseId,
            Title = userResponseAnalysis.Title,
            Score = userResponseAnalysis.Scores.Value.ToString() ?? "",
            Description = userResponseAnalysis.Description.Value.ToString() ?? ""
        };

        return userResponseAnalysisModel;
    }
    
    public List<GetAssessmentResponseAnalysisDto> GetUserResponseAnalysisDetailsForAssessments(Guid userResponseId)
    {
        var userResponse = genericRepository.GetById<UserResponse>(userResponseId)
                           ?? throw new NotFoundException("The following user response has not been saved.");
        
        return GetAssessmentResponseAnalysisDetails(userResponse.Id, true);
    }
    
    public List<GetAssessmentResponseAnalysisDto> GetUserResponseAnalysisEvaluationDetailsForAssessments(Guid questionnaireId, Guid userResponseId, bool isSubordinateRequired, int phase)
    {
        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
                            ?? throw new NotFoundException("The following questionnaire could not be found.");
        
        var userResponseModel = genericRepository.GetById<UserResponse>(userResponseId)
                        ?? throw new NotFoundException("The following candidate has not been registered to our system.");

        var userResponse = genericRepository.GetFirstOrDefault<UserResponse>(x => 
                               x.QuestionId == questionnaire.Id && x.CandidateId == userResponseModel.CandidateId && 
                               x.Phase == phase && x.IsAnsweredByCandidate)
                           ?? throw new NotFoundException("The following user response for the following questionnaire and the respective phase could not be found.");

        return GetAssessmentResponseAnalysisDetails(userResponse.Id, isSubordinateRequired);
    }

    private List<GetAssessmentResponseAnalysisDto> GetAssessmentResponseAnalysisDetails(Guid userResponseId, bool isSubordinateRequired)
    {
        var result = new List<GetAssessmentResponseAnalysisDto>();
        
        var userResponseAnalysis = genericRepository.GetFirstOrDefault<UserResponseAnalysis>(x => x.UserResponseId == userResponseId)
                                   ?? throw new NotFoundException("No user response has been saved for the following userResponseId.");

        var scores =
            keyValuePropertyService.GetProperty<List<InspectionResponseAnalysisDto>>(userResponseAnalysis.Scores);

        if (scores == null) return result;

        foreach (var score in scores)
        {
            var question = genericRepository.GetById<QuestionnaireDetails>(score.QuestionId)
                           ?? throw new NotFoundException("The following question has not been saved.");

            var responseAnalysis = new GetAssessmentResponseAnalysisDto()
            {
                QuestionId = question.Id,
                Question = question.Title,
                QuestionType = question.QuestionType.ToString(),
                Responses = isSubordinateRequired ? score.Responses : score.Responses.Where(x => x.Respondent == "Candidate").ToList()
            };
        
            result.Add(responseAnalysis);
        }

        return result;
    }
}