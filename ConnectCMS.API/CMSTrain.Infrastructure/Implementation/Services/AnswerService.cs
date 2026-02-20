using CMSTrain.Helper;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.DTOs.Answer;
using CMSTrain.Application.Common.User;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Analysis;
using CMSTrain.Application.DTOs.Certification;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;
using TrainingInspectionConfigurationModule = CMSTrain.Domain.Common.Enum.Configurations.TrainingInspectionConfiguration;
using TrainingInspectionConfigurationModel = CMSTrain.Application.DTOs.Configuration.TrainingInspection.TrainingInspectionConfiguration;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class AnswerService(IGenericRepository genericRepository, 
    ICurrentUserService userService, 
    ICertificationService certificationService,
    IKeyValuePropertyService keyValuePropertyService,
    ITrainingInspectionConfigurationService trainingInspectionConfigurationService) : IAnswerService
{
    public void UploadCandidateQuestionnaireAnswers(CandidateAnswerRequestDto candidateAnswers)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId) 
                        ?? throw new NotFoundException("The following user has not been registered to our system.");
        
        var questionnaire = genericRepository.GetById<Questionnaire>(candidateAnswers.QuestionnaireId)
            ?? throw new NotFoundException("The following questionnaire identifier is not valid.");

        if (questionnaire.TrainingInspectionId == null) throw new NotFoundException("The following questionnaire has not been assigned to a training.");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException("The following inspection could not be found.");
        
        var inspectionConfiguration =
            trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());

        if (inspectionConfiguration?.Accessibility == null) throw new NotFoundException("The following questionnaire does not have any configuration.");

        var phase = -1;

        var currentDate = ExtensionMethod.GetDateTimeInLocalTimeZone();

        for (var i = 0; i < inspectionConfiguration.Accessibility.Count; i++)
        {
            var accessibility = inspectionConfiguration.Accessibility[i];
            
            if (currentDate < accessibility.AccessPeriod || currentDate > accessibility.ExpirePeriod) continue;
            
            phase = i + 1; 
            
            break;
        }

        if (phase == -1) throw new BadRequestException("Your responses could not be stored.", ["No valid phase found for the current date."]);
        
        var userResponseDetails = new List<UserResponseDetails>();
        
        foreach (var answer in candidateAnswers.Answers)
        {
            var question = genericRepository.GetById<QuestionnaireDetails>(answer.QuestionId)
                           ?? throw new NotFoundException("The following question identifier is not valid.");

            if (question.QuestionType is QuestionType.SingleSelectMcq or QuestionType.MultiSelectMcq)
            {
                if (answer.AnswerId != null)
                {
                    foreach (var mcqAnswer in answer.AnswerId)
                    {
                        var answerModel = genericRepository.GetById<Answer>(mcqAnswer)
                            ?? throw new NotFoundException("The following provided multiple choice question answer is not valid.");
                        
                        userResponseDetails.Add(new UserResponseDetails()
                        {
                            AnswerId = answerModel.Id
                        });
                    }
                }
                else
                {
                    throw new NotFoundException("The following provided multiple choice question answer is not valid.");
                }
            }
            else
            {
                if (answer.Title is null)
                {
                    var exception = new[]
                    {
                        "Please provide valid answers for all of the following questions."
                    };
            
                    throw new BadRequestException("Your response could not be stored.", exception);
                }

                var answerModel = new Answer()
                {
                    QuestionId = question.Id,
                    Title = answer.Title,
                    IsSelectable = false
                };

                var answerId = genericRepository.Insert(answerModel);
                
                userResponseDetails.Add(new UserResponseDetails()
                {
                    AnswerId = answerId
                });
            }
        }

        var userResponse = new UserResponse()
        {
            QuestionId = questionnaire.Id,
            CandidateId = candidate.Id,
            AnsweredDate = DateTime.UtcNow,
            Remarks = candidateAnswers.Remarks,
            IsAnsweredByCandidate = true,
            IsAnsweredBySubordinate = false,
            Phase = phase,
            UserResponseDetails = userResponseDetails
        };

        var training = genericRepository.GetById<Training>(trainingInspection.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                                    x.TrainingId == training.Id && x.CandidateId == candidate.Id && x.IsApproved)
                                ?? throw new NotFoundException("The following candidate has not been assigned to the respective training.");
            
        certificationService.IssueTrainingCandidateCertification(new IssueCertificationDto()
        {
            TrainingCandidateId = trainingCandidate.Id
        });
        
        var userResponseId = genericRepository.Insert(userResponse);
        
        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The following inspection could not be found.");

        if (inspection.InspectionType == InspectionType.PersonalAssessment)
        {
            var subordinates = genericRepository.Get<Subordinate>(x => 
                    x.TrainingCandidateId == trainingCandidate.Id).ToList();
            
            var subordinateResponses = genericRepository.Get<UserResponse>(x => 
                x.SubordinateId != null && x.QuestionId == questionnaire.Id && 
                x.Phase == phase && x.IsAnsweredBySubordinate && 
                subordinates.Select(z => z.Id).Contains(x.SubordinateId.Value)).ToList();

            if (subordinateResponses.Count == 0)
            {
                var inspectionAnalysis = new List<InspectionResponseAnalysisDto>();
                
                foreach (var answer in candidateAnswers.Answers)
                {
                    var question = genericRepository.GetById<QuestionnaireDetails>(answer.QuestionId)
                        ?? throw new NotFoundException("The following question identifier is not valid.");
                    
                    inspectionAnalysis.Add(new InspectionResponseAnalysisDto()
                    {
                        QuestionId = answer.QuestionId,
                        QuestionType = question.QuestionType.ToString(),
                        Responses =
                        [
                            new ResponseAnalysis
                            {
                                Responses = question.QuestionType is QuestionType.LongQuestion or QuestionType.ShortQuestion ? answer.Title ?? "" : "",
                                Respondent = "Candidate",
                                Score = question.QuestionType == QuestionType.SingleSelectMcq
                                    ? GetScoreViaAnswerTitle(answer.AnswerId?.FirstOrDefault())
                                    : 0
                            }
                        ]
                    });

                    foreach (var subordinate in subordinates)
                    {
                        inspectionAnalysis.Last().Responses.Add(new ResponseAnalysis
                        {
                            Respondent = subordinate.SubordinateType.ToString(),
                            Responses = "",
                            Score = 0
                        });
                    }
                }
                
                var userResponseAnalysis = new UserResponseAnalysis()
                {
                    UserResponseId = userResponseId,
                    Title = "Personal Assessment - Traits and Evaluations",
                    Description = keyValuePropertyService.SaveProperty("Description", "Description"),
                    Scores = keyValuePropertyService.SaveProperty("Scores", inspectionAnalysis)
                };

                genericRepository.Insert(userResponseAnalysis);
            }
            else
            {
                var inspectionAnalysis = new List<InspectionResponseAnalysisDto>();
                
                foreach (var answer in candidateAnswers.Answers)
                {
                    var question = genericRepository.GetById<QuestionnaireDetails>(answer.QuestionId)
                        ?? throw new NotFoundException("The following question identifier is not valid.");
                    
                    inspectionAnalysis.Add(new InspectionResponseAnalysisDto()
                    {
                        QuestionId = answer.QuestionId,
                        QuestionType = question.QuestionType.ToString(),
                        Responses =
                        [
                            new ResponseAnalysis
                            {
                                Responses = question.QuestionType is QuestionType.LongQuestion or QuestionType.ShortQuestion ? answer.Title ?? "" : "",
                                Respondent = "Candidate",
                                Score = question.QuestionType == QuestionType.SingleSelectMcq
                                    ? GetScoreViaAnswerTitle(answer.AnswerId?.FirstOrDefault())
                                    : 0
                            }
                        ]
                    });

                    foreach (var subordinate in subordinates)
                    {
                        var subordinateResponse = genericRepository.GetFirstOrDefault<UserResponse>(x =>
                            x.SubordinateId != null && x.QuestionId == questionnaire.Id &&
                            x.Phase == phase && x.IsAnsweredBySubordinate && 
                            x.SubordinateId == subordinate.Id)
                            ?? throw new NotFoundException("The following response could not be found.");
                        
                        var answers = genericRepository.Get<Answer>(x => x.QuestionId == question.Id).ToList();
                        
                        var userResponseDetail = genericRepository.GetFirstOrDefault<UserResponseDetails>(x => 
                            x.UserResponseId == subordinateResponse.Id && answers.Select(z => z.Id).Contains(x.AnswerId))
                            ?? throw new NotFoundException("The following answer could not be found.");

                        var answerModel = genericRepository.GetById<Answer>(userResponseDetail.AnswerId)
                                          ?? throw new NotFoundException("The following answer could not be found.");
                        
                        inspectionAnalysis.Last().Responses.Add(new ResponseAnalysis
                        {
                            Respondent = subordinate.SubordinateType.ToString(),
                            Responses = question.QuestionType is QuestionType.LongQuestion or QuestionType.ShortQuestion ? answerModel.Title : "",
                            Score = question.QuestionType == QuestionType.SingleSelectMcq
                                ? GetScoreViaAnswerTitle(answerModel.Id)
                                : 0
                        });
                    }
                }
                
                var userResponseAnalysis = new UserResponseAnalysis()
                {
                    UserResponseId = userResponseId,
                    Title = "Personal Assessment - Traits and Evaluations",
                    Description = keyValuePropertyService.SaveProperty("Description", "Description"),
                    Scores = keyValuePropertyService.SaveProperty("Scores", inspectionAnalysis)
                };

                genericRepository.Insert(userResponseAnalysis);
            }
        }
    }

    public void UploadSubordinateQuestionnaireAnswers(SubordinateAnswerRequestDto subordinateAnswers)
    {
        var subordinateId = subordinateAnswers.SubordinateId;

        var subordinate = genericRepository.GetById<Subordinate>(subordinateId)
            ?? throw new NotFoundException("The following subordinate has not been registered to our system.");

        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(subordinate.TrainingCandidateId)
            ?? throw new NotFoundException("The following candidate has not assigned to the following training.");
    
        var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId)
            ?? throw new NotFoundException("The following candidate has not been registered to our system.");
        
        var questionnaire = genericRepository.GetById<Questionnaire>(subordinateAnswers.QuestionnaireId)
            ?? throw new NotFoundException("The following questionnaire identifier is not valid.");
        
        if (questionnaire.TrainingInspectionId == null) throw new NotFoundException("The following questionnaire has not been assigned to a training.");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException("The following inspection could not be found.");
        
        var inspectionConfiguration =
            trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());

        if (inspectionConfiguration?.Accessibility == null) throw new NotFoundException("The following questionnaire does not have any configuration.");

        var phase = -1;

        var currentDate = ExtensionMethod.GetDateTimeInLocalTimeZone();

        for (var i = 0; i < inspectionConfiguration.Accessibility.Count; i++)
        {
            var accessibility = inspectionConfiguration.Accessibility[i];
            
            if (currentDate < accessibility.AccessPeriod || currentDate > accessibility.ExpirePeriod) continue;
            
            phase = i + 1; 
            
            break;
        }

        if (phase == -1) throw new BadRequestException("Your responses could not be stored.", ["No valid phase found for the current date."]);

        var userResponseDetails = new List<UserResponseDetails>();
        
        foreach (var answer in subordinateAnswers.Answers)
        {
            var question = genericRepository.GetById<QuestionnaireDetails>(answer.QuestionId)
                ?? throw new NotFoundException("The following question identifier is not valid.");

            if (question.QuestionType is QuestionType.SingleSelectMcq or QuestionType.MultiSelectMcq)
            {
                if (answer.AnswerId != null)
                {
                    foreach (var mcqAnswer in answer.AnswerId)
                    {
                        var answerModel = genericRepository.GetById<Answer>(mcqAnswer)
                            ?? throw new NotFoundException("The following provided multiple choice question answer is not valid.");
                        
                        userResponseDetails.Add(new UserResponseDetails()
                        {
                            AnswerId = answerModel.Id
                        });
                    }
                }
                else
                {
                    throw new NotFoundException("The following provided multiple choice question answer is not valid.");
                }
            }
            else
            {
                if (answer.Title is null)
                {
                    var exception = new[]
                    {
                        "Please provide valid answers for all of the following questions."
                    };
            
                    throw new BadRequestException("Your response could not be stored.", exception);
                }

                var answerModel = new Answer()
                {
                    QuestionId = question.Id,
                    Title = answer.Title,
                    IsSelectable = false,
                    CreatedBy = candidate.Id
                };

                var answerId = genericRepository.Insert(answerModel);
                
                userResponseDetails.Add(new UserResponseDetails()
                {
                    AnswerId = answerId
                });
            }
        }

        userResponseDetails.ForEach(detail => detail.CreatedBy = candidate.Id);
        
        var userResponse = new UserResponse()
        {
            QuestionId = questionnaire.Id,
            CandidateId = candidate.Id,
            SubordinateId = subordinate.Id,
            AnsweredDate = DateTime.UtcNow,
            Remarks = subordinateAnswers.Remarks,
            IsAnsweredByCandidate = false,
            IsAnsweredBySubordinate = true,
            UserResponseDetails = userResponseDetails,
            CreatedBy = candidate.Id,
            Phase = phase
        };

        genericRepository.Insert(userResponse);
        
        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The following inspection could not be found.");
        
        if (inspection.InspectionType == InspectionType.PersonalAssessment)
        {
            var candidateResponse = genericRepository.GetFirstOrDefault<UserResponse>(x => 
                x.SubordinateId == null && x.QuestionId == questionnaire.Id && 
                x.Phase == phase && x.IsAnsweredByCandidate && x.CandidateId == candidate.Id);

            if (candidateResponse != null)
            {
                var analysis = genericRepository.GetFirstOrDefault<UserResponseAnalysis>(x => x.UserResponseId == candidateResponse.Id)
                    ?? throw new NotFoundException("The following response's analysis could not be found.");

                var analysisScores = keyValuePropertyService.GetProperty<List<InspectionResponseAnalysisDto>>(analysis.Scores);

                if (analysisScores != null)
                {
                    foreach (var analysisScore in analysisScores)
                    {
                        var answer = subordinateAnswers.Answers.First(x => x.QuestionId == analysisScore.QuestionId);
                        
                        var question = genericRepository.GetById<QuestionnaireDetails>(analysisScore.QuestionId)
                            ?? throw new NotFoundException("The following question could not be found.");
                        
                        var subordinateRespondent = analysisScore.Responses.FirstOrDefault(x => 
                            x.Respondent == subordinate.SubordinateType.ToString())
                            ?? throw new NotFoundException("The following response analysis could not be found.");
                        
                        subordinateRespondent.Responses = question.QuestionType is QuestionType.LongQuestion or QuestionType.ShortQuestion ? answer.Title ?? "" : "";
                        
                        subordinateRespondent.Score = question.QuestionType == QuestionType.SingleSelectMcq ? GetScoreViaAnswerTitle(answer.AnswerId?.FirstOrDefault()) : 0;
                    }
                    
                    analysis.Scores = keyValuePropertyService.SaveProperty("Scores", analysisScores);
                    
                    genericRepository.Update(analysis);
                }
            }
        }
    }
    
    public List<GetResponseUserDetails> GetResponseUserDetails(Guid questionnaireId, int phase)
    {
        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
            ?? throw new NotFoundException("The respective questionnaire could not be retrieved.");
        
        if (questionnaire.TrainingInspectionId == null) throw new NotFoundException("The following questionnaire has not been assigned to a training inspection.");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException(
                                     "The following questionnaire has not been linked to any of the inspection.");

        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The following inspection could not be found.");

        if (inspection.InspectionType != InspectionType.SwotAnalysis)
        {
            var userResponses = genericRepository.Get<UserResponse>(x => x.QuestionId == questionnaire.Id && x.Phase == phase && x.IsAnsweredByCandidate).ToList();

            return (from response in userResponses
                let user = genericRepository.GetById<User>(response.CandidateId) 
                           ?? throw new NotFoundException("The following candidate has not been registered to our system.")
                select new GetResponseUserDetails()
                {
                    UserResponseId = response.Id,
                    Id = user.Id,
                    Name = user.Name,
                    ImageUrl = user.ImageURL,
                    EmailAddress = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    AnsweredDate = response.AnsweredDate.ToFormattedDateTime()
                }).ToList();
        }
        
        var strategyResponses = genericRepository.Get<StrategicTraitResponse>(x => x.QuestionnaireId == questionnaire.Id && x.Phase == phase).ToList();
            
        return (from response in strategyResponses
            let user = genericRepository.GetById<User>(response.CandidateId) 
                       ?? throw new NotFoundException("The following candidate has not been registered to our system.")
            select new GetResponseUserDetails()
            {
                UserResponseId = response.Id,
                Id = user.Id,
                Name = user.Name,
                ImageUrl = user.ImageURL,
                EmailAddress = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                AnsweredDate = response.CreatedAt.ToFormattedDateTime()
            }).ToList();
    }
    
    public List<GetResponseUserDetails> GetResponseUserDetailsForClient(Guid questionnaireId, int phase)
    {
        var clientUserId = userService.GetUserId;
        
        var clientUser = genericRepository.GetById<User>(clientUserId)
            ?? throw new NotFoundException("The following user has not been registered to our system.");
        
        if (clientUser.OrganizationId == null) throw new NotFoundException("The following user is not registered to any client organization.");
        
        var organization = genericRepository.GetById<Organization>(clientUser.OrganizationId)
            ?? throw new NotFoundException("The following organization could not be found.");
        
        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
            ?? throw new NotFoundException("The respective questionnaire could not be retrieved.");
        
        if (questionnaire.TrainingInspectionId == null) throw new NotFoundException("The following questionnaire has not been assigned to a training inspection.");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException(
                                     "The following questionnaire has not been linked to any of the inspection.");

        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The following inspection could not be found.");

        if (inspection.InspectionType != InspectionType.SwotAnalysis)
        {
            var userResponses = genericRepository.Get<UserResponse>(x => x.QuestionId == questionnaire.Id && x.Phase == phase && x.IsAnsweredByCandidate).ToList();

            return (from response in userResponses
                let user = genericRepository.GetById<User>(response.CandidateId) 
                           ?? throw new NotFoundException("The following candidate has not been registered to our system.")
                where user.OrganizationId == organization.Id
                select new GetResponseUserDetails()
                {
                    UserResponseId = response.Id,
                    Id = user.Id,
                    Name = user.Name,
                    ImageUrl = user.ImageURL,
                    EmailAddress = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    AnsweredDate = response.AnsweredDate.ToFormattedDateTime()
                }).ToList();
        }
        
        var strategyResponses = genericRepository.Get<StrategicTraitResponse>(x => x.QuestionnaireId == questionnaire.Id && x.Phase == phase).ToList();
            
        return (from response in strategyResponses
            let user = genericRepository.GetById<User>(response.CandidateId) 
                       ?? throw new NotFoundException("The following candidate has not been registered to our system.")
            where user.OrganizationId == organization.Id
            select new GetResponseUserDetails()
            {
                UserResponseId = response.Id,
                Id = user.Id,
                Name = user.Name,
                ImageUrl = user.ImageURL,
                EmailAddress = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                AnsweredDate = response.CreatedAt.ToFormattedDateTime()
            }).ToList();
    }
    
    public GetAnswerDetailsDto GetQuestionAnswerDetails(Guid userResponseId)
    {
        var userResponse = genericRepository.GetById<UserResponse>(userResponseId)
            ?? throw new NotFoundException("The respective user's responses could not be retrieved.");

        var userResponseDetails = 
            genericRepository.Get<UserResponseDetails>(x => 
                x.UserResponseId == userResponse.Id)
            .ToList();

        var questionnaire = genericRepository.GetById<Questionnaire>(userResponse.QuestionId)
            ?? throw new NotFoundException("The respective questionnaire could not be retrieved.");

        if (questionnaire.TrainingInspectionId == null) throw new NotFoundException("The respective questionnaire has not been assigned to a training.");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException(
                                     "The following questionnaire has not been assigned to a training.");
        
        var questionDetails =
            genericRepository.Get<QuestionnaireDetails>(x => 
                x.QuestionnaireId == questionnaire.Id).OrderBy(x => x.Order).ToList();

        var questionAnswers = new List<GetQuestionAnswerDetailsDto>();

        foreach (var questionDetail in questionDetails)
        {
            var heading = string.Empty;
            
            if (questionDetail.HeadingId != null)
            {
                if (questionDetail.IsParentHeading == true)
                {
                    var headingModel = genericRepository.GetById<Heading>(questionDetail.HeadingId)
                        ?? throw new NotFoundException("The following heading could not be found.");
                    
                    heading = headingModel.Title;
                }
                else
                {
                    var subheadingModel = genericRepository.GetById<Heading>(questionDetail.HeadingId)
                                       ?? throw new NotFoundException("The following heading could not be found.");
                    
                    if(subheadingModel.ParentHeadingId == null) throw new NotFoundException("The following heading could not be found.");
                    
                    var headingModel = genericRepository.GetById<Heading>(subheadingModel.ParentHeadingId)
                                       ?? throw new NotFoundException("The following heading could not be found.");

                    heading = $"{headingModel.Title} > {subheadingModel.Title}";
                }
            }
            
            var responseDetails = new List<QuestionAnswerDetails>();
            
            var selectedAnswerIds = userResponseDetails
                .Where(x => genericRepository.GetById<Answer>(x.AnswerId)?.QuestionId == questionDetail.Id)
                .Select(x => x.AnswerId)
                .ToList();

            if (questionDetail.QuestionType is QuestionType.Rating or QuestionType.LongQuestion or QuestionType.ShortQuestion)
            {
                var userAnswer = userResponseDetails
                    .FirstOrDefault(x => 
                        genericRepository.GetById<Answer>(x.AnswerId)?.QuestionId == questionDetail.Id);
                    
                if (userAnswer != null)
                {
                    var answer = genericRepository.GetById<Answer>(userAnswer.AnswerId)
                        ?? throw new NotFoundException("The following answer could not be found.");

                    responseDetails.Add(new QuestionAnswerDetails
                    {
                        Id = answer.Id,
                        Rating = questionDetail.QuestionType is QuestionType.Rating ? Convert.ToInt32(answer.Title) : 0,
                        Title = answer.Title,
                        IsSelectable = false,
                        IsSelected = true
                    });
                }
            }
            else
            {
                var answers = genericRepository.Get<Answer>(x => x.QuestionId == questionDetail.Id).OrderBy(x => x.Order).ToList();

                responseDetails = answers.Select(x => new QuestionAnswerDetails()
                {
                    Id = x.Id,
                    Title = x.Title,
                    IsSelectable = true,
                    IsSelected = selectedAnswerIds.Contains(x.Id)
                }).ToList();
            }

            var questionAnswerDetails = new GetQuestionAnswerDetailsDto()
            {
                QuestionId = questionDetail.Id,
                Heading = heading,
                Title = questionDetail.Title,
                QuestionType = questionDetail.QuestionType.ToString(),
                Answers = responseDetails
            };
            
            questionAnswers.Add(questionAnswerDetails);
        }

        return new GetAnswerDetailsDto()
        {
            Id = userResponse.Id,
            QuestionnaireId = questionnaire.Id,
            TrainingInspectionId = trainingInspection.Id,
            Phase = userResponse.Phase,
            Remarks = userResponse.Remarks,
            AnsweredDate = userResponse.AnsweredDate.ToFormattedDateTime(),
            CandidateId = userResponse.CandidateId,
            SubordinateId = userResponse.SubordinateId,
            IsAnsweredByCandidate = userResponse.IsAnsweredByCandidate,
            IsAnsweredBySubordinate = userResponse.IsAnsweredBySubordinate,
            QuestionAnswers = questionAnswers
        };
    }

    public GetUserResponseDto GetUserResponseDetails(Guid userResponseId)
    {
        var userResponse = genericRepository.GetById<UserResponse>(userResponseId)
            ?? throw new NotFoundException("The respective user's responses could not be retrieved.");
        
        var questionnaire = genericRepository.GetById<Questionnaire>(userResponse.QuestionId)
            ?? throw new NotFoundException("The respective questionnaire could not be retrieved.");

        return new GetUserResponseDto()
        {
            UserResponseId = userResponse.Id,
            QuestionnaireId = questionnaire.Id
        };
    }
    
    public GeneralQuestionnaireAnswerResponseDto GetGeneralQuestionnaireAnswerResponses(Guid questionnaireId, int phase)
    {
        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
            ?? throw new NotFoundException("The respective questionnaire could not be retrieved.");
        
        var questionDetails = genericRepository.Get<QuestionnaireDetails>(x => x.QuestionnaireId == questionnaire.Id).OrderBy(x => x.Order).ToList();

        var result = new GeneralQuestionnaireAnswerResponseDto()
        {
            TotalResponses = genericRepository.GetCount<UserResponse>(x => x.QuestionId == questionnaire.Id && x.IsAnsweredByCandidate && x.Phase == phase),
            GeneralAnswers = []
        };
        
        var generalAnswerResponses = new List<GeneralAnswerResponseDto>();

        foreach (var questionDetail in questionDetails)
        {
            var answerResponseDetailsList = new List<AnswerResponseDetails>();

            var answers = genericRepository.Get<Answer>(x => x.QuestionId == questionDetail.Id).OrderBy(x => x.Order).ToList();

            var totalResponses = genericRepository
                .GetCount<UserResponseDetails>(x => x.Answer.QuestionId == questionDetail.Id && x.UserResponse.Phase == phase);
            
            if (questionDetail.QuestionType is QuestionType.MultiSelectMcq or QuestionType.SingleSelectMcq)
            {
                foreach (var answer in answers)
                {
                    var selectedCount = genericRepository
                        .GetCount<UserResponseDetails>(x => x.AnswerId == answer.Id && x.UserResponse.Phase == phase);

                    var percentage = totalResponses > 0 ? (decimal)selectedCount / totalResponses * 100 : 0;

                    answerResponseDetailsList.Add(new AnswerResponseDetails
                    {
                        AnswerId = answer.Id,
                        Answer = answer.Title,
                        Count = selectedCount,
                        Percentage = (double) percentage
                    });
                }
            }
            else if (questionDetail.QuestionType is QuestionType.Rating)
            {
                var answerGroup = answers
                    .GroupBy(x => x.Title)
                    .Select(g => new 
                    {
                        Title = g.Key,
                        Count = genericRepository
                            .Get<UserResponseDetails>(x => x.Answer.QuestionId == questionDetail.Id && x.Answer.Title == g.Key && x.UserResponse.Phase == phase)
                            .Count()
                    })
                    .ToList();

                foreach (var group in answerGroup)
                {
                    var percentage = totalResponses > 0 ? (decimal)group.Count / totalResponses * 100 : 0;

                    answerResponseDetailsList.Add(new AnswerResponseDetails
                    {
                        AnswerId = Guid.NewGuid(),
                        Answer = group.Title,
                        Count = group.Count,
                        Percentage = (double) percentage
                    });
                }
            }
            else
            {
                var userResponseDetails = genericRepository
                    .Get<UserResponseDetails>(x => x.Answer.QuestionId == questionDetail.Id && x.UserResponse.Phase == phase)
                    .ToList();

                foreach (var userResponseDetail in userResponseDetails)
                {
                    var answer = genericRepository.GetById<Answer>(userResponseDetail.AnswerId);
                    
                    if (answer == null) continue;

                    answerResponseDetailsList.Add(new AnswerResponseDetails
                    {
                        AnswerId = answer.Id,
                        Answer = answer.Title,
                        Count = 1,
                        Percentage = 100
                    });
                }
            }

            generalAnswerResponses.Add(new GeneralAnswerResponseDto
            {
                QuestionId = questionDetail.Id,
                Title = questionDetail.Title,
                QuestionType = questionDetail.QuestionType.ToString(),
                Answers = answerResponseDetailsList
            });
        }

        result.GeneralAnswers = generalAnswerResponses;
        
        return result;
    }

    public int GetQuestionnaireAnswerResponseCount(GeneralQuestionAnswerResponseDto generalQuestionAnswerResponse)
    {
        var answer = genericRepository.GetFirstOrDefault<Answer>(x => x.QuestionId == generalQuestionAnswerResponse.QuestionId && x.Title == generalQuestionAnswerResponse.AnswerTitle);
        
        if (answer == null) return 0;

        var userResponse = genericRepository.Get<UserResponse>(x => x.QuestionId == generalQuestionAnswerResponse.QuestionnaireId && x.Phase == generalQuestionAnswerResponse.Phase).ToList();

        return genericRepository.GetCount<UserResponseDetails>(x => x.AnswerId == answer.Id && userResponse.Select(z => z.Id).Contains(x.UserResponseId));
    }
    
    private double GetScoreViaAnswerTitle(Guid? answerId)
    {
        if (answerId == null) return 0;
        
        var answerModel = genericRepository.GetById<Answer>(answerId)
                          ?? throw new NotFoundException("The following answer could not be found.");
        
        return answerModel.Title switch
        {
            "Strongly Disagree" => 1,
            "Disagree" => 2,
            "Neutral" => 3,
            "Agree" => 4,
            "Strongly Agree" => 5,
            _ => 0
        };
    }
}
