using ClosedXML.Excel;
using CMSTrain.Helper;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using Microsoft.Extensions.Options;
using CMSTrain.Application.Settings;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Common.User;
using CMSTrain.Application.DTOs.Answer;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Questionnaires;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;
using TrainingInspectionConfigurationModule = CMSTrain.Domain.Common.Enum.Configurations.TrainingInspectionConfiguration;
using TrainingInspectionConfigurationModel = CMSTrain.Application.DTOs.Configuration.TrainingInspection.TrainingInspectionConfiguration;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class QuestionnaireService(
    IAnswerService answerService,
    IQrCodeService qrCodeService,
    ICurrentUserService userService,
    ITrainingService trainingService,
    IInspectionService inspectionService,
    IGenericRepository genericRepository,
    IPersonalityTestService personalityTestService,
    ITrainingInspectionConfigurationService trainingInspectionConfigurationService,
    IOptions<ClientSettings> clientSettings) : IQuestionnaireService
{
    private readonly string _baseUrl = clientSettings.Value.BaseUrl.Split(";").FirstOrDefault()
                                       ?? throw new NotFoundException(
                                           "The Base URL has not been stabilized and initialized");

    public GetQuestionnaireDetailsDto GetQuestionnaireModuleDetails(Guid questionnaireId)
    {
        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
                            ?? throw new NotFoundException("The following questionnaire could not be found.");

        if (questionnaire.TrainingInspectionId == null)
            throw new BadRequestException("Questionnaire details could not be found.",
                ["The following questionnaire does not have a training inspection assigned to it."]);

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException(
                                     "The following training inspection could not be found.");

        var training = genericRepository.GetById<Training>(trainingInspection.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The following training could not be found.");

        return new GetQuestionnaireDetailsDto()
        {
            QuestionnaireId = questionnaire.Id,
            InspectionId = inspection.Id,
            TrainingId = training.Id,
            TrainingInspectionId = trainingInspection.Id
        };
    }

    public GetQuestionnaireDto GetQuestionnaireDetails(Guid questionnaireId)
    {
        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
                            ?? throw new NotFoundException("The respective questionnaire could not be found.");

        var trainingInspection = questionnaire.TrainingInspectionId != null
            ? genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
              ?? throw new NotFoundException(
                  "The following questionnaire has not been assigned to the respective training inspection module.")
            : throw new NotFoundException(
                "The respective questionnaire does not have an allocated training inspection.");

        var questionnaireDetails =
            genericRepository.Get<QuestionnaireDetails>(x =>
                x.QuestionnaireId == questionnaire.Id).OrderBy(x => x.Order).ToList();

        var headingsDictionary = new Dictionary<Guid, HeadingQuestionDetailsDto>();

        var subHeadingsDictionary = new Dictionary<Guid, SubHeadingDto>();

        var result = new GetQuestionnaireDto()
        {
            QuestionnaireId = questionnaire.Id,
            TrainingInspectionId = trainingInspection.Id,
            IsQuestionnaireForTraining = true,
        };

        foreach (var questionnaireDetail in questionnaireDetails)
        {
            var questionTrait = genericRepository.GetFirstOrDefault<QuestionnaireTraits>(x =>
                x.QuestionId == questionnaireDetail.Id);

            var questionDetail = new GetQuestionDetailsDto
            {
                QuestionId = questionnaireDetail.Id,
                Title = questionnaireDetail.Title,
                Type = questionnaireDetail.QuestionType.ToString(),
                Trait = questionTrait?.TraitType.ToString() ?? "",
                Answers = GetAnswersForQuestion(questionnaireDetail)
            };

            if (questionnaireDetail.HeadingId.HasValue)
            {
                var headingId = questionnaireDetail.HeadingId.Value;

                if (questionnaireDetail.IsParentHeading == true)
                {
                    if (!headingsDictionary.ContainsKey(headingId))
                    {
                        var heading = genericRepository.GetById<Heading>(headingId)
                                      ?? throw new NotFoundException("The following heading could not be found.");

                        headingsDictionary[headingId] = new HeadingQuestionDetailsDto
                        {
                            HeadingId = heading.Id,
                            Heading = heading.Title,
                            Questions = new List<GetQuestionDetailsDto>(),
                            SubHeadingQuestions = new List<SubHeadingDto>()
                        };

                        result.HeadingQuestions.Add(headingsDictionary[headingId]);
                    }

                    headingsDictionary[headingId].Questions.Add(questionDetail);
                }
                else
                {
                    var subHeading = genericRepository.GetById<Heading>(headingId)
                                     ?? throw new NotFoundException("The following sub heading could not be found.");

                    var parentHeadingId = subHeading.ParentHeadingId
                                          ?? throw new BadRequestException("Questionnaire details could not be found",
                                              ["The sub-heading does not have a parent heading."]);

                    if (!subHeadingsDictionary.ContainsKey(headingId))
                    {
                        subHeadingsDictionary[headingId] = new SubHeadingDto
                        {
                            HeadingId = subHeading.Id,
                            Heading = subHeading.Title,
                            Questions = []
                        };

                        if (!headingsDictionary.ContainsKey(parentHeadingId))
                        {
                            var parentHeading = genericRepository.GetById<Heading>(parentHeadingId)
                                                ?? throw new NotFoundException(
                                                    "The following heading could not be found.");

                            headingsDictionary[parentHeadingId] = new HeadingQuestionDetailsDto
                            {
                                HeadingId = parentHeading.Id,
                                Heading = parentHeading.Title,
                                Questions = [],
                                SubHeadingQuestions = []
                            };
                            result.HeadingQuestions.Add(headingsDictionary[parentHeadingId]);
                        }

                        headingsDictionary[parentHeadingId].SubHeadingQuestions.Add(subHeadingsDictionary[headingId]);
                    }

                    subHeadingsDictionary[headingId].Questions.Add(questionDetail);
                }
            }
            else
            {
                result.Questions.Add(questionDetail);
            }
        }

        return result;
    }
    
    public GetQuestionnaireDto GetAllQuestionnairesForTrainingInspection(Guid trainingInspectionId)
    {
        var trainingInspection = genericRepository.GetById<TrainingInspection>(trainingInspectionId)
                                 ?? throw new NotFoundException(
                                     "The following inspection has not been assigned to the following training.");

        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The following inspection could not be found.");

        var questionnaire = genericRepository.GetFirstOrDefault<Questionnaire>(x =>
            x.TrainingInspectionId == trainingInspection.Id && x.IsQuestionnaireForTraining);

        if (questionnaire == null) return GetAllQuestionnairesFromInspectionUpload(inspection.Id);

        var result = new GetQuestionnaireDto()
        {
            QuestionnaireId = questionnaire.Id,
            IsQuestionnaireForTraining = true,
            TrainingInspectionId = trainingInspection.Id,
            Questions = [],
            HeadingQuestions = []
        };

        var questionnaireDetails = genericRepository.Get<QuestionnaireDetails>(x =>
            x.QuestionnaireId == questionnaire.Id).OrderBy(x => x.Order).ToList();

        var headingsDictionary = new Dictionary<Guid, HeadingQuestionDetailsDto>();

        var subHeadingsDictionary = new Dictionary<Guid, SubHeadingDto>();

        foreach (var questionnaireDetail in questionnaireDetails)
        {
            var questionTrait = genericRepository.GetFirstOrDefault<QuestionnaireTraits>(x =>
                x.QuestionId == questionnaireDetail.Id);

            var questionDetail = new GetQuestionDetailsDto
            {
                QuestionId = questionnaireDetail.Id,
                Title = questionnaireDetail.Title,
                Type = questionnaireDetail.QuestionType.ToString(),
                Trait = questionTrait?.TraitType.ToString() ?? "",
                Answers = GetAnswersForQuestion(questionnaireDetail, inspection.Id),
            };

            if (questionnaireDetail.HeadingId.HasValue)
            {
                var headingId = questionnaireDetail.HeadingId.Value;

                if (questionnaireDetail.IsParentHeading == true)
                {
                    if (!headingsDictionary.ContainsKey(headingId))
                    {
                        var heading = genericRepository.GetById<Heading>(headingId)
                                      ?? throw new NotFoundException("The following heading could not be found.");
                        headingsDictionary[headingId] = new HeadingQuestionDetailsDto
                        {
                            HeadingId = heading.Id,
                            Heading = heading.Title,
                            Questions = new List<GetQuestionDetailsDto>(),
                            SubHeadingQuestions = new List<SubHeadingDto>()
                        };

                        result.HeadingQuestions.Add(headingsDictionary[headingId]);
                    }

                    headingsDictionary[headingId].Questions.Add(questionDetail);
                }
                else
                {
                    var subHeading = genericRepository.GetById<Heading>(headingId)
                                     ?? throw new NotFoundException("The following sub heading could not be found.");

                    var parentHeadingId = subHeading.ParentHeadingId
                                          ?? throw new BadRequestException("Questionnaire details could not be found",
                                              ["The sub-heading does not have a parent heading."]);

                    if (!subHeadingsDictionary.ContainsKey(headingId))
                    {
                        subHeadingsDictionary[headingId] = new SubHeadingDto
                        {
                            HeadingId = subHeading.Id,
                            Heading = subHeading.Title,
                            Questions = []
                        };

                        if (!headingsDictionary.ContainsKey(parentHeadingId))
                        {
                            var parentHeading = genericRepository.GetById<Heading>(parentHeadingId)
                                                ?? throw new NotFoundException(
                                                    "The following heading could not be found.");
                            headingsDictionary[parentHeadingId] = new HeadingQuestionDetailsDto
                            {
                                HeadingId = parentHeading.Id,
                                Heading = parentHeading.Title,
                                Questions = [],
                                SubHeadingQuestions = []
                            };
                            result.HeadingQuestions.Add(headingsDictionary[parentHeadingId]);
                        }

                        headingsDictionary[parentHeadingId].SubHeadingQuestions.Add(subHeadingsDictionary[headingId]);
                    }

                    subHeadingsDictionary[headingId].Questions.Add(questionDetail);
                }
            }
            else
            {
                result.Questions.Add(questionDetail);
            }
        }

        return result;
    }

    // During view of personal assessments and when questions are to be uploaded for it on a respective training
    public GetQuestionnaireDto GetAllQuestionnairesFromInspectionUpload(Guid inspectionId)
    {
        var inspection = genericRepository.GetById<Inspection>(inspectionId)
                         ?? throw new NotFoundException("The following inspection could not be found.");

        var inspectionQuestionnaire = genericRepository.GetFirstOrDefault<InspectionQuestionnaires>(x =>
            x.InspectionId == inspection.Id);

        if (inspectionQuestionnaire == null)
        {
            return new GetQuestionnaireDto();
        }

        var questionnaire = genericRepository.GetById<Questionnaire>(inspectionQuestionnaire.QuestionnaireId)
                            ?? throw new NotFoundException("The following questionnaire could not be found.");

        var questionnaireDetails = genericRepository.Get<QuestionnaireDetails>(x =>
            x.QuestionnaireId == questionnaire.Id).OrderBy(x => x.Order).ToList();

        var answers = genericRepository.Get<Answer>(x =>
            x.IsAnswerForInspection && x.InspectionId == inspection.Id).ToList();

        var questionnaireDto = new GetQuestionnaireDto
        {
            QuestionnaireId = questionnaire.Id,
            IsQuestionnaireForTraining = questionnaire.IsQuestionnaireForTraining,
            HeadingQuestions = [],
            Questions = [],
            PredefinedAnswers = answers.OrderBy(x => x.Order).Select(x => new McqAnswerDetailsDto()
            {
                Title = x.Title,
                QuestionType = x.AnswerType.ToString()
            }).ToList()
        };

        var headingsDictionary = new Dictionary<Guid, HeadingQuestionDetailsDto>();

        var subHeadingsDictionary = new Dictionary<Guid, SubHeadingDto>();

        foreach (var questionnaireDetail in questionnaireDetails)
        {
            var questionTrait = genericRepository.GetFirstOrDefault<QuestionnaireTraits>(x =>
                x.QuestionId == questionnaireDetail.Id);

            var questionDetail = new GetQuestionDetailsDto
            {
                QuestionId = questionnaireDetail.Id,
                Title = questionnaireDetail.Title,
                Type = questionnaireDetail.QuestionType.ToString(),
                Trait = questionTrait?.TraitType.ToString() ?? "",
                Answers = GetAnswersForQuestion(questionnaireDetail, inspection.Id)
            };

            if (questionnaireDetail.HeadingId.HasValue)
            {
                var headingId = questionnaireDetail.HeadingId.Value;

                if (questionnaireDetail.IsParentHeading == true)
                {
                    if (!headingsDictionary.ContainsKey(headingId))
                    {
                        var heading = genericRepository.GetById<Heading>(headingId)
                                      ?? throw new NotFoundException("The following heading could not be found.");

                        headingsDictionary[headingId] = new HeadingQuestionDetailsDto
                        {
                            HeadingId = heading.Id,
                            Heading = heading.Title,
                            Questions = new List<GetQuestionDetailsDto>(),
                            SubHeadingQuestions = new List<SubHeadingDto>()
                        };

                        questionnaireDto.HeadingQuestions.Add(headingsDictionary[headingId]);
                    }

                    headingsDictionary[headingId].Questions.Add(questionDetail);
                }
                else
                {
                    var subHeading = genericRepository.GetById<Heading>(headingId)
                                     ?? throw new NotFoundException("The following sub heading could not be found.");

                    var parentHeadingId = subHeading.ParentHeadingId
                                          ?? throw new BadRequestException("Questionnaire details could not be found",
                                              ["The sub-heading does not have a parent heading."]);

                    if (!subHeadingsDictionary.ContainsKey(headingId))
                    {
                        subHeadingsDictionary[headingId] = new SubHeadingDto
                        {
                            HeadingId = subHeading.Id,
                            Heading = subHeading.Title,
                            Questions = []
                        };

                        if (!headingsDictionary.ContainsKey(parentHeadingId))
                        {
                            var parentHeading = genericRepository.GetById<Heading>(parentHeadingId)
                                                ?? throw new NotFoundException(
                                                    "The following heading could not be found.");
                            headingsDictionary[parentHeadingId] = new HeadingQuestionDetailsDto
                            {
                                HeadingId = parentHeading.Id,
                                Heading = parentHeading.Title,
                                Questions = [],
                                SubHeadingQuestions = []
                            };

                            questionnaireDto.HeadingQuestions.Add(headingsDictionary[parentHeadingId]);
                        }

                        headingsDictionary[parentHeadingId].SubHeadingQuestions.Add(subHeadingsDictionary[headingId]);
                    }

                    subHeadingsDictionary[headingId].Questions.Add(questionDetail);
                }
            }
            else
            {
                questionnaireDto.Questions.Add(questionDetail);
            }
        }

        return questionnaireDto;
    }

    public GetCandidateQuestionnaireDto GetAllQuestionnairesForCandidate(Guid questionnaireId)
    {
        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
                            ?? throw new NotFoundException("The respective questionnaire could not be found.");

        if (questionnaire.TrainingInspectionId == null)
            throw new NotFoundException(
                "The following questionnaire doesn't have a training inspection assigned to it.");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException(
                                     "The following inspection has not been assigned to the respective questionnaire.");

        var questionnaireDetails =
            genericRepository.Get<QuestionnaireDetails>(x =>
                x.QuestionnaireId == questionnaire.Id).OrderBy(x => x.Order).ToList();

        if (questionnaireDetails.Count == 0) return new GetCandidateQuestionnaireDto();

        var headingsDictionary = new Dictionary<Guid, HeadingQuestionDetailsDto>();

        var subHeadingsDictionary = new Dictionary<Guid, SubHeadingDto>();

        var result = new GetCandidateQuestionnaireDto()
        {
            TrainingInspectionId = trainingInspection.Id,
            QuestionnaireId = questionnaire.Id
        };

        foreach (var questionnaireDetail in questionnaireDetails)
        {
            var questionTrait = genericRepository.GetFirstOrDefault<QuestionnaireTraits>(x =>
                x.QuestionId == questionnaireDetail.Id);

            var questionDetail = new GetQuestionDetailsDto
            {
                QuestionId = questionnaireDetail.Id,
                Title = questionnaireDetail.Title,
                Type = questionnaireDetail.QuestionType.ToString(),
                Trait = questionTrait?.TraitType.ToString() ?? "",
                Answers = GetAnswersForQuestion(questionnaireDetail)
            };

            if (questionnaireDetail.HeadingId.HasValue)
            {
                var headingId = questionnaireDetail.HeadingId.Value;

                if (questionnaireDetail.IsParentHeading == true)
                {
                    if (!headingsDictionary.ContainsKey(headingId))
                    {
                        var heading = genericRepository.GetById<Heading>(headingId)
                                      ?? throw new NotFoundException("The following heading could not be found.");

                        headingsDictionary[headingId] = new HeadingQuestionDetailsDto
                        {
                            HeadingId = heading.Id,
                            Heading = heading.Title,
                            Questions = new List<GetQuestionDetailsDto>(),
                            SubHeadingQuestions = new List<SubHeadingDto>()
                        };

                        result.HeadingQuestions.Add(headingsDictionary[headingId]);
                    }

                    headingsDictionary[headingId].Questions.Add(questionDetail);
                }
                else
                {
                    var subHeading = genericRepository.GetById<Heading>(headingId)
                                     ?? throw new NotFoundException("The following sub heading could not be found.");

                    var parentHeadingId = subHeading.ParentHeadingId
                                          ?? throw new BadRequestException("Questionnaire details could not be found",
                                              ["The sub-heading does not have a parent heading."]);

                    if (!subHeadingsDictionary.ContainsKey(headingId))
                    {
                        subHeadingsDictionary[headingId] = new SubHeadingDto
                        {
                            HeadingId = subHeading.Id,
                            Heading = subHeading.Title,
                            Questions = []
                        };

                        if (!headingsDictionary.ContainsKey(parentHeadingId))
                        {
                            var parentHeading = genericRepository.GetById<Heading>(parentHeadingId)
                                                ?? throw new NotFoundException(
                                                    "The following heading could not be found.");

                            headingsDictionary[parentHeadingId] = new HeadingQuestionDetailsDto
                            {
                                HeadingId = parentHeading.Id,
                                Heading = parentHeading.Title,
                                Questions = [],
                                SubHeadingQuestions = []
                            };
                            result.HeadingQuestions.Add(headingsDictionary[parentHeadingId]);
                        }

                        headingsDictionary[parentHeadingId].SubHeadingQuestions.Add(subHeadingsDictionary[headingId]);
                    }

                    subHeadingsDictionary[headingId].Questions.Add(questionDetail);
                }
            }
            else
            {
                result.Questions.Add(questionDetail);
            }
        }

        return result;
    }

    public GetCandidateQuestionnaireDto GetAllQuestionnairesForSubordinates(Guid questionnaireId, Guid subordinateId)
    {
        var subordinate = genericRepository.GetById<Subordinate>(subordinateId)
                          ?? throw new NotFoundException(
                              "The following subordinate with the specified identifier was not found.");

        var trainingCandidate =
            genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                x.Id == subordinate.TrainingCandidateId && x.IsApproved)
            ?? throw new NotFoundException("The following candidate has not been accepted to the respective training.");

        var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
                            ?? throw new NotFoundException("The respective questionnaire could not be found.");

        if (questionnaire.TrainingInspectionId == null)
            throw new NotFoundException(
                "The following questionnaire doesn't have a training inspection assigned to it.");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException(
                                     "The following inspection has not been assigned to the respective questionnaire.");

        var questionnaireDetails =
            genericRepository.Get<QuestionnaireDetails>(x =>
                x.QuestionnaireId == questionnaire.Id).OrderBy(x => x.Order).ToList();

        if (questionnaireDetails.Count == 0) return new GetCandidateQuestionnaireDto();

        var headingsDictionary = new Dictionary<Guid, HeadingQuestionDetailsDto>();

        var subHeadingsDictionary = new Dictionary<Guid, SubHeadingDto>();

        var result = new GetCandidateQuestionnaireDto()
        {
            TrainingInspectionId = trainingInspection.Id,
            QuestionnaireId = questionnaire.Id
        };

        foreach (var questionnaireDetail in questionnaireDetails)
        {
            var questionTrait = genericRepository.GetFirstOrDefault<QuestionnaireTraits>(x =>
                x.QuestionId == questionnaireDetail.Id);

            var (pronoun, possessivePronoun, reflexivePronoun) = candidate.Gender.GetPronouns();

            var questionTitle = questionnaireDetail.Title
                .Replace(" I ", $" {pronoun} ")
                .Replace(" my ", $" {possessivePronoun} ")
                .Replace(" me ", $" {pronoun} ")
                .Replace(" myself", $" {reflexivePronoun}")
                .Replace("My ", $"{char.ToUpper(possessivePronoun[0])}{possessivePronoun[1..]} ")
                .Replace("I ", $"{char.ToUpper(pronoun[0])}{pronoun[1..]} ");

            var questionDetail = new GetQuestionDetailsDto
            {
                QuestionId = questionnaireDetail.Id,
                Title = questionTitle,
                Type = questionnaireDetail.QuestionType.ToString(),
                Trait = questionTrait?.TraitType.ToString() ?? "",
                Answers = GetAnswersForQuestion(questionnaireDetail)
            };

            if (questionnaireDetail.HeadingId.HasValue)
            {
                var headingId = questionnaireDetail.HeadingId.Value;

                if (questionnaireDetail.IsParentHeading == true)
                {
                    if (!headingsDictionary.ContainsKey(headingId))
                    {
                        var heading = genericRepository.GetById<Heading>(headingId)
                                      ?? throw new NotFoundException("The following heading could not be found.");

                        headingsDictionary[headingId] = new HeadingQuestionDetailsDto
                        {
                            HeadingId = heading.Id,
                            Heading = heading.Title,
                            Questions = new List<GetQuestionDetailsDto>(),
                            SubHeadingQuestions = new List<SubHeadingDto>()
                        };

                        result.HeadingQuestions.Add(headingsDictionary[headingId]);
                    }

                    headingsDictionary[headingId].Questions.Add(questionDetail);
                }
                else
                {
                    var subHeading = genericRepository.GetById<Heading>(headingId)
                                     ?? throw new NotFoundException("The following sub heading could not be found.");

                    var parentHeadingId = subHeading.ParentHeadingId
                                          ?? throw new BadRequestException("Questionnaire details could not be found",
                                              ["The sub-heading does not have a parent heading."]);

                    if (!subHeadingsDictionary.ContainsKey(headingId))
                    {
                        subHeadingsDictionary[headingId] = new SubHeadingDto
                        {
                            HeadingId = subHeading.Id,
                            Heading = subHeading.Title,
                            Questions = []
                        };

                        if (!headingsDictionary.ContainsKey(parentHeadingId))
                        {
                            var parentHeading = genericRepository.GetById<Heading>(parentHeadingId)
                                                ?? throw new NotFoundException(
                                                    "The following heading could not be found.");

                            headingsDictionary[parentHeadingId] = new HeadingQuestionDetailsDto
                            {
                                HeadingId = parentHeading.Id,
                                Heading = parentHeading.Title,
                                Questions = [],
                                SubHeadingQuestions = []
                            };
                            result.HeadingQuestions.Add(headingsDictionary[parentHeadingId]);
                        }

                        headingsDictionary[parentHeadingId].SubHeadingQuestions.Add(subHeadingsDictionary[headingId]);
                    }

                    subHeadingsDictionary[headingId].Questions.Add(questionDetail);
                }
            }
            else
            {
                result.Questions.Add(questionDetail);
            }
        }

        return result;
    }

    // TODO: Debug the values of answers from the uploaded questionnaire and inspection questionnaires
    // TODO: Make a separate entity for evaluation of common answers
    public void UploadQuestionnaires(QuestionnaireUploadDto questionnaire)
    {
        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException(
                                     "The respective assigned training inspection could not be found.");

        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The following inspection could not be found.");

        var questions = new Questionnaire
        {
            TrainingInspectionId = trainingInspection.Id,
            IsQuestionnaireForTraining = true,
            QuestionDetails = questionnaire.QuestionDetails.Select((x, index) => new QuestionnaireDetails()
            {
                Title = x.Title,
                QuestionType = x.Type,
                HeadingId = x.HeadingId,
                Order = index,
                IsParentHeading = x.IsParentHeading,
                HasUniqueAnswers = true,
                Answers = x.Answers.Select((z, order) => new Answer()
                {
                    IsAnswerForInspection = false,
                    IsAnswerForQuestion = true,
                    InspectionId = inspection.Id,
                    Order = order,
                    Title = z.Title,
                    IsSelectable = inspection.InspectionType == InspectionType.PersonalityTest ||
                                   x.Type is QuestionType.SingleSelectMcq or QuestionType.MultiSelectMcq
                }).ToList(),
                QuestionnaireTraits = inspection.InspectionType == InspectionType.PersonalityTest
                    ? x.TraitTypes?.Select(z => new QuestionnaireTraits()
                    {
                        TraitType = z
                    }).ToList()
                    : [],
            }).ToList()
        };

        genericRepository.Insert(questions);
    }

    public GetQuestionnaireValidityDto GetQuestionnaireValidity(Guid questionnaireId)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
                            ?? throw new NotFoundException("The following questionnaire could not be found.");

        if (questionnaire.TrainingInspectionId == null)
            throw new NotFoundException("The following questionnaire has not been assigned any training.");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException(
                                     "The following training inspection could not be found.");

        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The following inspection could not be found.");

        var inspectionConfiguration =
            trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());

        bool isAnswered;

        if (inspection.InspectionType == InspectionType.SwotAnalysis)
        {
            var userResponse = genericRepository.GetFirstOrDefault<StrategicTraitResponse>(x =>
                x.QuestionnaireId == questionnaire.Id && x.CandidateId == candidate.Id);

            isAnswered = userResponse != null;
        }
        else
        {
            var userResponse = genericRepository.GetFirstOrDefault<UserResponse>(x =>
                x.QuestionId == questionnaire.Id && x.CandidateId == candidate.Id && x.IsAnsweredByCandidate);

            isAnswered = userResponse != null;
        }

        if (inspectionConfiguration?.Accessibility == null)
            throw new NotFoundException("The following questionnaire does not have any configuration.");

        var phase = -1;

        var currentDate = ExtensionMethod.GetDateTimeInLocalTimeZone();

        for (var i = 0; i < inspectionConfiguration.Accessibility.Count; i++)
        {
            var accessibility = inspectionConfiguration.Accessibility[i];

            if (currentDate < accessibility.AccessPeriod || currentDate > accessibility.ExpirePeriod) continue;

            phase = i + 1;

            break;
        }

        return new GetQuestionnaireValidityDto()
        {
            IsAnswered = isAnswered,
            IsValid = phase != -1,
        };
    }
    
    public byte[] ExportQuestionnaireDetails(Guid questionnaireId, int phase)
    {
        #region Module Entity Data Representation
        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
                            ?? throw new NotFoundException("The following questionnaire could not be found.");

        if (questionnaire.TrainingInspectionId == null)
            throw new NotFoundException("The following inspection has not been assigned to the respective training.");
        
        var trainingInspections = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                  ?? throw new NotFoundException(
                                      "The following training inspection could not be found.");

        var inspection = genericRepository.GetById<Inspection>(trainingInspections.InspectionId)
                         ?? throw new NotFoundException("The following inspections could not be found.");
        #endregion

        #region Setup of ClosedXML Workbook
        var workbook = new XLWorkbook();
        #endregion

        #region Data Population
        
        if (inspection.InspectionType == InspectionType.PersonalityTest)
        {
            var personalityTestQuestionnaire = personalityTestService.GetPersonalityTestQuestionnaires(questionnaire.Id, false);

            #region Page 1: Questions and Statistcis

            #region Initialization of Worksheet
            var questionnaireRow = 1;
            var questionsSheet = workbook.Worksheets.Add("Questions");

            questionsSheet.Column(1).Width = 10;
            questionsSheet.Column(2).Width = 75;
            questionsSheet.Column(3).Width = 75;
            questionsSheet.Column(4).Width = 50;
            questionsSheet.Column(5).Width = 50;
            questionsSheet.Column(6).Width = 30;
            questionsSheet.Column(7).Width = 30;
            questionsSheet.Column(8).Width = 30;
            questionsSheet.Column(9).Width = 30;
            questionsSheet.Column(10).Width = 30;
            questionsSheet.Column(11).Width = 100;
            #endregion
            
            #region Questions Table Header
            questionsSheet.Cell(questionnaireRow, 1).Value = "#";
            questionsSheet.Cell(questionnaireRow, 2).Value = "Question Title";
            questionsSheet.Cell(questionnaireRow, 3).Value = "Strongly Disagree";
            questionsSheet.Cell(questionnaireRow, 4).Value = "Disagree";
            questionsSheet.Cell(questionnaireRow, 5).Value = "Neutral";
            questionsSheet.Cell(questionnaireRow, 6).Value = "Agree";
            questionsSheet.Cell(questionnaireRow, 7).Value = "Strongly Agree";

            for (var col = 1; col <= 7; col++)
            {
                var cell = questionsSheet.Cell(questionnaireRow, col);
                
                cell.Style.Font.Bold = true;

                if (col is >= 3 and <= 7)
                {
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            }
            
            questionnaireRow++;
            #endregion

            #region Personality Traits and Questions with Facets
            var traitIndex = 1;
            var facetIndex = 1;
            
            foreach (var questionnaireTraits in personalityTestQuestionnaire.QuestionnaireTraits)
            {
                questionsSheet.Cell(questionnaireRow, 1).Value = $"{traitIndex}.";
                
                var traitCellRange = questionsSheet.Range(questionnaireRow, 2, questionnaireRow, 7);
                
                traitCellRange.Merge();
                traitCellRange.Value = questionnaireTraits.Trait;
                traitCellRange.Style.Font.Italic = true;

                var traitRowRange = questionsSheet.Range(questionnaireRow, 1, questionnaireRow, 7);
                traitRowRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#F1973C");

                questionnaireRow++;
                
                foreach (var facet in questionnaireTraits.Facets)
                {
                    questionsSheet.Cell(questionnaireRow, 1).Value = $"{traitIndex}.{facetIndex}.";
                    
                    var facetCellRange = questionsSheet.Range(questionnaireRow, 2, questionnaireRow, 7);
                
                    facetCellRange.Merge();
                    facetCellRange.Value = facet.Facet;
                    facetCellRange.Style.Font.Italic = true;

                    var facetRowRange = questionsSheet.Range(questionnaireRow, 1, questionnaireRow, 7);
                    facetRowRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFE8C8");
                    
                    questionnaireRow++;

                    var questionIndex = 1;

                    foreach (var question in facet.Questions)
                    {
                        questionsSheet.Cell(questionnaireRow, 1).Value = $"{traitIndex}.{facetIndex}.{questionIndex}.";
                        questionsSheet.Cell(questionnaireRow, 2).Value = question.Title;

                        var personalityTestAnswers = new List<string>()
                        {
                            "Strongly Disagree",
                            "Disagree",
                            "Neutral",
                            "Agree",
                            "Strongly Agree"
                        };

                        foreach (var personalityTestAnswer in personalityTestAnswers)
                        {
                            var index = personalityTestAnswers.IndexOf(personalityTestAnswer);
                            
                            var generalQuestionAnswerResponse = new GeneralQuestionAnswerResponseDto()
                            {
                                QuestionnaireId = questionnaire.Id,
                                QuestionId = question.QuestionId,
                                Phase = phase,
                                AnswerTitle = personalityTestAnswer
                            };
                        
                            var answerCount = answerService.GetQuestionnaireAnswerResponseCount(generalQuestionAnswerResponse);
                            
                            questionsSheet.Cell(questionnaireRow, index + 3).Value = answerCount;
                        }

                        questionnaireRow++;
                        questionIndex++;
                    }

                    facetIndex++;
                }

                traitIndex++;
            }
            #endregion

            #region Finalization of Worksheet
            questionsSheet.Style.Font.FontName = "Aptos";

            var questionsCellRange = questionsSheet.RangeUsed();

            if (questionsCellRange != null)
            {
                foreach (var cell in questionsCellRange.Cells())
                {
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }
            }
            #endregion
            
            #endregion
        }
        else if (inspection.InspectionType is InspectionType.PersonalAssessment or InspectionType.Feedback)
        {
            var questionnaireDetails = GetAllQuestionnairesForCandidate(questionnaireId);

            #region Page 1: Questions and Statistics

            #region Initialization of Worksheet
            var questionnaireRow = 1;
            var questionsSheet = workbook.Worksheets.Add("Questions");

            questionsSheet.Column(1).Width = 10;
            questionsSheet.Column(2).Width = 100;
            questionsSheet.Column(3).Width = 20;
            questionsSheet.Column(4).Width = 20;
            questionsSheet.Column(5).Width = 20;
            questionsSheet.Column(6).Width = 20;
            questionsSheet.Column(7).Width = 20;
            questionsSheet.Column(8).Width = 20;
            #endregion

            #region Questions Table Header
            questionsSheet.Cell(questionnaireRow, 1).Value = "#";
            questionsSheet.Cell(questionnaireRow, 2).Value = "Question Title";
            questionsSheet.Cell(questionnaireRow, 4).Value = "Question Type";

            var answerCellRange = questionsSheet.Range(questionnaireRow, 4, questionnaireRow, 8);
            answerCellRange.Merge();
            answerCellRange.Value = "Answers";
            answerCellRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            
            for (var col = 1; col <= 8; col++)
            {
                var cell = questionsSheet.Cell(questionnaireRow, col);
                cell.Style.Font.Bold = true;
            }
            
            questionnaireRow++;
            #endregion
            
            #region Questions with / without Heading and Sub-Heading
            var headingIndex = 1;
            var questionIndex = 1;
            
            foreach (var question in questionnaireDetails.Questions)
            {
                questionsSheet.Cell(questionnaireRow, 1).Value = $"{questionIndex}.";
                questionsSheet.Cell(questionnaireRow, 2).Value = question.Title;
                questionsSheet.Cell(questionnaireRow, 3).Value = GetQuestionType(question.Type);

                if (question.Type == QuestionType.MultiSelectMcq.ToString() || question.Type == QuestionType.SingleSelectMcq.ToString())
                {
                    foreach (var answer in question.Answers)
                    {
                        var answerIndex = question.Answers.IndexOf(answer);
                        
                        var generalQuestionAnswerResponse = new GeneralQuestionAnswerResponseDto()
                        {
                            QuestionnaireId = questionnaire.Id,
                            QuestionId = question.QuestionId,
                            Phase = phase,
                            AnswerTitle = answer.Title
                        };
                        
                        var answerCount = answerService.GetQuestionnaireAnswerResponseCount(generalQuestionAnswerResponse);

                        questionsSheet.Cell(questionnaireRow, answerIndex + 4).Value = $"{answer.Title} ({answerCount})";
                    }
                }
                else if (question.Type == QuestionType.Rating.ToString())
                {
                    for (var i = 0; i < 5; i++)
                    {
                        var ratingAnswer = $"{i + 1}";
                            
                        var generalQuestionAnswerResponse = new GeneralQuestionAnswerResponseDto()
                        {
                            QuestionnaireId = questionnaire.Id,
                            QuestionId = question.QuestionId,
                            Phase = phase,
                            AnswerTitle = ratingAnswer
                        };
                        
                        var answerCount = answerService.GetQuestionnaireAnswerResponseCount(generalQuestionAnswerResponse);

                        questionsSheet.Cell(questionnaireRow, i + 4).Value = $"{ratingAnswer} ({answerCount})";
                    }
                }
                else
                {
                    var answerDetailsCellRange = questionsSheet.Range(questionnaireRow, 4, questionnaireRow, 8);
                    answerDetailsCellRange.Merge();
                    answerDetailsCellRange.Value = "-";
                    answerDetailsCellRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                questionIndex++;
                questionnaireRow++;
            }

            foreach (var headingQuestion in questionnaireDetails.HeadingQuestions)
            {
                questionsSheet.Cell(questionnaireRow, 1).Value = $"{headingIndex}.";
                
                var headingCellRange = questionsSheet.Range(questionnaireRow, 2, questionnaireRow, 8);
                
                headingCellRange.Merge();
                headingCellRange.Value = headingQuestion.Heading;
                headingCellRange.Style.Font.Italic = true;

                var traitRowRange = questionsSheet.Range(questionnaireRow, 1, questionnaireRow, 7);
                traitRowRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#F1973C");

                questionnaireRow++;

                var headingQuestionIndex = 1;

                foreach (var question in headingQuestion.Questions)
                {
                    questionsSheet.Cell(questionnaireRow, 1).Value = $"{headingIndex}.0.{headingQuestionIndex}.";
                    questionsSheet.Cell(questionnaireRow, 2).Value = question.Title;
                    questionsSheet.Cell(questionnaireRow, 3).Value = GetQuestionType(question.Type);

                    if (question.Type == QuestionType.MultiSelectMcq.ToString() || question.Type == QuestionType.SingleSelectMcq.ToString())
                    {
                        foreach (var answer in question.Answers)
                        {
                            var answerIndex = question.Answers.IndexOf(answer);
                        
                            var generalQuestionAnswerResponse = new GeneralQuestionAnswerResponseDto()
                            {
                                QuestionnaireId = questionnaire.Id,
                                QuestionId = question.QuestionId,
                                Phase = phase,
                                AnswerTitle = answer.Title
                            };
                        
                            var answerCount = answerService.GetQuestionnaireAnswerResponseCount(generalQuestionAnswerResponse);

                            questionsSheet.Cell(questionnaireRow, answerIndex + 4).Value = $"{answer.Title} ({answerCount})";
                        }
                    }
                    else if (question.Type == QuestionType.Rating.ToString())
                    {
                        for (var i = 0; i < 5; i++)
                        {
                            var ratingAnswer = $"{i + 1}";
                            
                            var generalQuestionAnswerResponse = new GeneralQuestionAnswerResponseDto()
                            {
                                QuestionnaireId = questionnaire.Id,
                                QuestionId = question.QuestionId,
                                Phase = phase,
                                AnswerTitle = ratingAnswer
                            };
                        
                            var answerCount = answerService.GetQuestionnaireAnswerResponseCount(generalQuestionAnswerResponse);

                            questionsSheet.Cell(questionnaireRow, i + 4).Value = $"{ratingAnswer} ({answerCount})";
                        }
                    }
                    else
                    {
                        var answerDetailsCellRange = questionsSheet.Range(questionnaireRow, 4, questionnaireRow, 8);
                        answerDetailsCellRange.Merge();
                        answerDetailsCellRange.Value = "-";
                        answerDetailsCellRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    questionnaireRow++;
                    headingQuestionIndex++;
                }

                var subHeadingIndex = 1;

                foreach (var subHeadingQuestion in headingQuestion.SubHeadingQuestions)
                {
                    questionsSheet.Cell(questionnaireRow, 1).Value = $"{headingIndex}.{subHeadingIndex}.";
                    
                    var subHeadingCellRange = questionsSheet.Range(questionnaireRow, 2, questionnaireRow, 8);
                
                    subHeadingCellRange.Merge();
                    subHeadingCellRange.Value = subHeadingQuestion.Heading;
                    subHeadingCellRange.Style.Font.Italic = true;

                    var subHeadingRowRange = questionsSheet.Range(questionnaireRow, 1, questionnaireRow, 7);
                    subHeadingRowRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFE8C8");

                    questionnaireRow++;
                    
                    var subHeadingQuestionIndex = 1;

                    foreach (var question in subHeadingQuestion.Questions)
                    {
                        questionsSheet.Cell(questionnaireRow, 1).Value = $"{headingIndex}.{subHeadingIndex}.{subHeadingQuestionIndex}.";
                        questionsSheet.Cell(questionnaireRow, 2).Value = question.Title;
                        questionsSheet.Cell(questionnaireRow, 3).Value = GetQuestionType(question.Type);

                        if (question.Type == QuestionType.MultiSelectMcq.ToString() || question.Type == QuestionType.SingleSelectMcq.ToString())
                        {
                            foreach (var answer in question.Answers)
                            {
                                var answerIndex = question.Answers.IndexOf(answer);
                        
                                var generalQuestionAnswerResponse = new GeneralQuestionAnswerResponseDto()
                                {
                                    QuestionnaireId = questionnaire.Id,
                                    QuestionId = question.QuestionId,
                                    Phase = phase,
                                    AnswerTitle = answer.Title
                                };
                        
                                var answerCount = answerService.GetQuestionnaireAnswerResponseCount(generalQuestionAnswerResponse);

                                questionsSheet.Cell(questionnaireRow, answerIndex + 4).Value = $"{answer.Title} ({answerCount})";
                            }
                        }
                        else if (question.Type == QuestionType.Rating.ToString())
                        {
                            for (var i = 0; i < 5; i++)
                            {
                                var ratingAnswer = $"{i + 1}";
                            
                                var generalQuestionAnswerResponse = new GeneralQuestionAnswerResponseDto()
                                {
                                    QuestionnaireId = questionnaire.Id,
                                    QuestionId = question.QuestionId,
                                    Phase = phase,
                                    AnswerTitle = ratingAnswer
                                };
                        
                                var answerCount = answerService.GetQuestionnaireAnswerResponseCount(generalQuestionAnswerResponse);

                                questionsSheet.Cell(questionnaireRow, i + 4).Value = $"{ratingAnswer} ({answerCount})";
                            }
                        }
                        else
                        {
                            var answerDetailsCellRange = questionsSheet.Range(questionnaireRow, 4, questionnaireRow, 8);
                            answerDetailsCellRange.Merge();
                            answerDetailsCellRange.Value = "-";
                            answerDetailsCellRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        questionnaireRow++;
                        subHeadingQuestionIndex++;
                    }

                    subHeadingIndex++;
                }

                headingIndex++;
            }
            #endregion
            
            #region Finalization of Worksheet
            questionsSheet.Style.Font.FontName = "Aptos";

            var questionsCellRange = questionsSheet.RangeUsed();

            if (questionsCellRange != null)
            {
                foreach (var cell in questionsCellRange.Cells())
                {
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }
            }
            #endregion
            
            #endregion
        }

        var userResponseDetails = answerService.GetResponseUserDetails(questionnaireId, phase);
        
        #region Page 1 / 2: Respondents
    
        #region Initialization of Worksheet
        var respondentRowValue = 1;
        var respondentsSheet = workbook.Worksheets.Add("Respondents");

        respondentsSheet.Column(1).Width = 10;
        respondentsSheet.Column(2).Width = 50;
        respondentsSheet.Column(3).Width = 75;
        respondentsSheet.Column(4).Width = 20;
        respondentsSheet.Column(5).Width = 20;
        #endregion
        
        #region Questions Table Header
        respondentsSheet.Cell(respondentRowValue, 1).Value = "#";
        respondentsSheet.Cell(respondentRowValue, 2).Value = "Name";
        respondentsSheet.Cell(respondentRowValue, 3).Value = "Email Address";
        respondentsSheet.Cell(respondentRowValue, 4).Value = "Phone Number";
        respondentsSheet.Cell(respondentRowValue, 5).Value = "Answered Date";

        for (var col = 1; col <= 5; col++)
        {
            var cell = respondentsSheet.Cell(respondentRowValue, col);
            cell.Style.Font.Bold = true;
        }
        
        respondentRowValue++;
        #endregion
        
        #region Respondent Details
        foreach (var userResponse in userResponseDetails)
        {
            respondentsSheet.Cell(respondentRowValue, 1).Value = $"{userResponseDetails.IndexOf(userResponse) + 1}.";
            respondentsSheet.Cell(respondentRowValue, 2).Value = userResponse.Name;
            respondentsSheet.Cell(respondentRowValue, 3).Value = userResponse.EmailAddress;
            respondentsSheet.Cell(respondentRowValue, 4).Value = userResponse.PhoneNumber;
            respondentsSheet.Cell(respondentRowValue, 5).Value = userResponse.AnsweredDate;

            respondentRowValue++;
        }
        #endregion

        #region Finalization of Worksheet
        respondentsSheet.Style.Font.FontName = "Aptos";

        var respondentsCellRange = respondentsSheet.RangeUsed();

        if (respondentsCellRange != null)
        {
            foreach (var cell in respondentsCellRange.Cells())
            {
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }
        }
        #endregion
        
        #endregion

        if (inspection.InspectionType != InspectionType.SwotAnalysis)
        {
            AddQuestionAnswerMatrixSheet(workbook, questionnaireId, userResponseDetails, inspection.InspectionType);
        }
        
        #endregion

        #region Assignment of Workbook
        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        #endregion
        
        return memoryStream.ToArray();
    }

    public GetTrainingQuestionnaireDto GetTrainingQuestionnaireDetails(Guid questionnaireId)
    {
        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
                            ?? throw new NotFoundException("The following questionnaire could not be found.");

        if (questionnaire.TrainingInspectionId == null)
            throw new NotFoundException("The following inspection has not been assigned to the respective training.");
        
        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                  ?? throw new NotFoundException(
                                      "The following training inspection could not be found.");

        var training = trainingService.GetTrainingById(trainingInspection.TrainingId);
        
        var inspection = inspectionService.GetInspectionById(trainingInspection.InspectionId);

        return new GetTrainingQuestionnaireDto()
        {
            Training = training,
            Inspection = inspection
        };
    }
    
    public byte[] GenerateQuestionnaireAnswerUploadFormQrCode(Guid questionnaireId, string inspectionType)
    {
        var inspection = inspectionType.ToInspectionTypeModel();

        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
                            ?? throw new NotFoundException("The respective questionnaire could not be found.");

        var answerUploadForm = inspection switch
        {
            InspectionType.PersonalityTest => Constants.Navigation.PersonalityTestQuestionnaireAnswerUploadForm,
            InspectionType.SwotAnalysis => Constants.Navigation.StrategicTraitQuestionnaireAnswerUploadForm,
            InspectionType.Feedback => Constants.Navigation.FeedbackAssessmentQuestionnaireAnswerUploadForm,
            InspectionType.PersonalAssessment => Constants.Navigation.FeedbackAssessmentQuestionnaireAnswerUploadForm,
            InspectionType.Others => Constants.Navigation.FeedbackAssessmentQuestionnaireAnswerUploadForm,
            InspectionType.None => throw new ArgumentException($"Unknown inspection type value: {inspection}"),
            _ => throw new ArgumentException($"Unknown inspection type value: {inspection}")
        };

        var answerUploadFormNavigation = $"{_baseUrl}/{answerUploadForm}/{questionnaire.Id}";

        var base64QrCode = qrCodeService.GenerateQrCode(answerUploadFormNavigation);

        return Convert.FromBase64String(base64QrCode);
    }
    
    private List<AnswerDetails> GetAnswersForQuestion(QuestionnaireDetails questionnaireDetail, Guid? inspectionId = null)
    {
        List<AnswerDetails> answers;

        if (questionnaireDetail.HasUniqueAnswers)
        {
            var questionDetailAnswers = genericRepository.Get<Answer>(x =>
                x.IsAnswerForQuestion && x.QuestionId == questionnaireDetail.Id).ToList();

            answers = questionDetailAnswers.OrderBy(x => x.Order).Select(x => new AnswerDetails
            {
                Id = x.Id,
                Title = x.Title,
                IsSelectable = x.IsSelectable
            }).ToList();
        }
        else
        {
            var questionDetailAnswers = genericRepository.Get<Answer>(x =>
                x.IsAnswerForInspection && x.InspectionId == inspectionId &&
                x.AnswerType == questionnaireDetail.QuestionType).ToList();

            answers = questionDetailAnswers.OrderBy(x => x.Order).Select(x => new AnswerDetails
            {
                Id = x.Id,
                Title = x.Title,
                IsSelectable = x.IsSelectable
            }).ToList();
        }

        return answers;
    }

    private void AddQuestionAnswerMatrixSheet(XLWorkbook workbook, Guid questionnaireId,
        List<GetResponseUserDetails> userResponseDetails, InspectionType inspectionType)
    {
        var answerMatrixRow = 1;
        var answerMatrixSheet = workbook.Worksheets.Add("Answer Analysis");
        var includeAnalysisColumns = inspectionType == InspectionType.Feedback;
        var questionHeaders = GetQuestionMatrixHeaders(questionnaireId, inspectionType);

        answerMatrixSheet.Column(1).Width = 10;
        answerMatrixSheet.Column(2).Width = 35;
        answerMatrixSheet.Column(3).Width = 35;
        answerMatrixSheet.Column(4).Width = 20;
        answerMatrixSheet.Column(5).Width = 22;

        answerMatrixSheet.Cell(answerMatrixRow, 1).Value = "#";
        answerMatrixSheet.Cell(answerMatrixRow, 2).Value = "Candidate Name";
        answerMatrixSheet.Cell(answerMatrixRow, 3).Value = "Email Address";
        answerMatrixSheet.Cell(answerMatrixRow, 4).Value = "Phone Number";
        answerMatrixSheet.Cell(answerMatrixRow, 5).Value = "Answered Date";

        var firstQuestionColumnIndex = 6;

        if (includeAnalysisColumns)
        {
            answerMatrixSheet.Column(6).Width = 30;
            answerMatrixSheet.Column(7).Width = 20;
            answerMatrixSheet.Column(8).Width = 60;

            answerMatrixSheet.Cell(answerMatrixRow, 6).Value = "Analysis Title";
            answerMatrixSheet.Cell(answerMatrixRow, 7).Value = "Analysis Score";
            answerMatrixSheet.Cell(answerMatrixRow, 8).Value = "Analysis Description";

            firstQuestionColumnIndex = 9;
        }

        foreach (var questionHeader in questionHeaders)
        {
            var columnIndex = questionHeaders.IndexOf(questionHeader) + firstQuestionColumnIndex;
            answerMatrixSheet.Cell(answerMatrixRow, columnIndex).Value = questionHeader.Header;
            answerMatrixSheet.Column(columnIndex).Width = 35;
        }

        var totalColumns = firstQuestionColumnIndex + questionHeaders.Count - 1;

        for (var col = 1; col <= totalColumns; col++)
        {
            answerMatrixSheet.Cell(answerMatrixRow, col).Style.Font.Bold = true;
        }

        answerMatrixRow++;

        foreach (var userResponse in userResponseDetails)
        {
            var questionAnswerDetails = answerService.GetQuestionAnswerDetails(userResponse.UserResponseId);
            var questionAnswers = questionAnswerDetails.QuestionAnswers
                .ToDictionary(x => x.QuestionId, x => x);

            answerMatrixSheet.Cell(answerMatrixRow, 1).Value = $"{userResponseDetails.IndexOf(userResponse) + 1}.";
            answerMatrixSheet.Cell(answerMatrixRow, 2).Value = userResponse.Name;
            answerMatrixSheet.Cell(answerMatrixRow, 3).Value = userResponse.EmailAddress;
            answerMatrixSheet.Cell(answerMatrixRow, 4).Value = userResponse.PhoneNumber;
            answerMatrixSheet.Cell(answerMatrixRow, 5).Value = userResponse.AnsweredDate;

            if (includeAnalysisColumns)
            {
                var responseAnalysis = genericRepository.GetFirstOrDefault<UserResponseAnalysis>(x =>
                    x.UserResponseId == userResponse.UserResponseId);

                answerMatrixSheet.Cell(answerMatrixRow, 6).Value =
                    string.IsNullOrWhiteSpace(responseAnalysis?.Title) ? "-" : responseAnalysis.Title;
                answerMatrixSheet.Cell(answerMatrixRow, 7).Value = responseAnalysis?.Scores?.Value?.ToString() ?? "-";
                answerMatrixSheet.Cell(answerMatrixRow, 8).Value = responseAnalysis?.Description?.Value?.ToString() ?? "-";
            }

            foreach (var questionHeader in questionHeaders)
            {
                var columnIndex = questionHeaders.IndexOf(questionHeader) + firstQuestionColumnIndex;
                answerMatrixSheet.Cell(answerMatrixRow, columnIndex).Value =
                    questionAnswers.TryGetValue(questionHeader.QuestionId, out var questionAnswer)
                        ? GetQuestionAnswerAnalysisValue(questionAnswer)
                        : "-";
            }

            answerMatrixRow++;
        }

        answerMatrixSheet.Style.Font.FontName = "Aptos";
        answerMatrixSheet.Style.Alignment.WrapText = true;

        var answerMatrixCellRange = answerMatrixSheet.RangeUsed();

        if (answerMatrixCellRange != null)
        {
            foreach (var cell in answerMatrixCellRange.Cells())
            {
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }
        }
    }

    private List<(Guid QuestionId, string Header)> GetQuestionMatrixHeaders(Guid questionnaireId,
        InspectionType inspectionType)
    {
        var questionHeaders = new List<(Guid QuestionId, string Header)>();

        if (inspectionType == InspectionType.PersonalityTest)
        {
            var personalityTestQuestionnaire = personalityTestService.GetPersonalityTestQuestionnaires(questionnaireId, false);

            foreach (var questionnaireTrait in personalityTestQuestionnaire.QuestionnaireTraits)
            {
                foreach (var facet in questionnaireTrait.Facets)
                {
                    foreach (var question in facet.Questions)
                    {
                        questionHeaders.Add((question.QuestionId,
                            $"{questionnaireTrait.Trait} - {facet.Facet} - {question.Title}"));
                    }
                }
            }

            return questionHeaders;
        }

        var questionnaireDetails = GetAllQuestionnairesForCandidate(questionnaireId);

        foreach (var question in questionnaireDetails.Questions)
        {
            questionHeaders.Add((question.QuestionId, question.Title));
        }

        foreach (var headingQuestion in questionnaireDetails.HeadingQuestions)
        {
            foreach (var question in headingQuestion.Questions)
            {
                questionHeaders.Add((question.QuestionId, $"{headingQuestion.Heading} - {question.Title}"));
            }

            foreach (var subHeadingQuestion in headingQuestion.SubHeadingQuestions)
            {
                foreach (var question in subHeadingQuestion.Questions)
                {
                    questionHeaders.Add((question.QuestionId,
                        $"{headingQuestion.Heading} - {subHeadingQuestion.Heading} - {question.Title}"));
                }
            }
        }

        return questionHeaders;
    }

    private static string GetQuestionAnswerAnalysisValue(GetQuestionAnswerDetailsDto questionAnswer)
    {
        var selectedAnswers = questionAnswer.Answers.Where(x => x.IsSelected).ToList();

        if (selectedAnswers.Count == 0) return "-";

        if (questionAnswer.QuestionType == QuestionType.Rating.ToString())
        {
            var selectedRating = selectedAnswers.First();
            return selectedRating.Rating > 0 ? selectedRating.Rating.ToString() : selectedRating.Title;
        }

        var answerValues = selectedAnswers
            .Select(x => x.Title)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        return answerValues.Count == 0 ? "-" : string.Join(", ", answerValues);
    }
    private string GetQuestionType(string questionType)
    {
        return questionType switch
        {
            not null when questionType == QuestionType.SingleSelectMcq.ToString() => "Single Select MCQ",
            not null when questionType == QuestionType.MultiSelectMcq.ToString() => "Multi Select MCQ",
            not null when questionType == QuestionType.Rating.ToString() => "Rating",
            not null when questionType == QuestionType.LongQuestion.ToString() => "Long Question",
            not null when questionType == QuestionType.ShortQuestion.ToString() => "Short Question",
            _ => "None"
        };
    }
}
