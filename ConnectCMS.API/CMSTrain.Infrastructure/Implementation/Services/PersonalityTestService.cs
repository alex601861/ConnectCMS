using CMSTrain.Helper;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Common.User;
using CMSTrain.Application.DTOs.Answer;
using CMSTrain.Application.DTOs.Certification;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Questionnaires;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.PersonalityTest;
using CMSTrain.Application.Interfaces.Repositories.Base;
using TrainingInspectionConfigurationModule = CMSTrain.Domain.Common.Enum.Configurations.TrainingInspectionConfiguration;
using TrainingInspectionConfigurationModel = CMSTrain.Application.DTOs.Configuration.TrainingInspection.TrainingInspectionConfiguration;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class PersonalityTestService(IGenericRepository genericRepository, 
    ITrainingInspectionConfigurationService trainingInspectionConfigurationService,
    IKeyValuePropertyService keyValuePropertyService,
    ICertificationService certificationService,
    ICurrentUserService userService) 
    : IPersonalityTestService
{
    public GetPersonalityTestQuestionnaireDto GetPersonalityTestQuestionnaires(Guid questionnaireId, bool isRandomizedDataRequired)
    {
        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
            ?? throw new NotFoundException("The following questionnaire could not be found.");

        if (questionnaire.TrainingInspectionId == null)
            throw new NotFoundException("The following questionnaire is not linked to a training inspection.");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
            ?? throw new NotFoundException("The following questionnaire is not linked to a training inspection.");

        var questionDetails = genericRepository.Get<QuestionnaireDetails>(x => 
            x.QuestionnaireId == questionnaire.Id).OrderBy(x => x.Order).ToList();

        var allQuestionnaireTraits = genericRepository.Get<QuestionnaireTraits>(x =>
            questionDetails.Select(z => z.Id).Contains(x.QuestionId)).ToList();

        var facetHeadings = genericRepository.Get<Heading>(x =>
            x.Facet == FacetType.Facet && 
            x.Type == HeadingType.Heading &&
            x.Inspection == InspectionType.PersonalityTest).ToList();

        var personalityTestQuestionnaire = new GetPersonalityTestQuestionnaireDto
        {
            QuestionnaireId = questionnaire.Id,
            TrainingInspectionId = trainingInspection.Id,
            QuestionnaireTraits = new List<QuestionnaireTrait>()
        };

        foreach (TraitType trait in Enum.GetValues(typeof(TraitType)))
        {
            var traitQuestions = allQuestionnaireTraits
                .Where(qt => qt.TraitType == trait)
                .Select(qt => qt.QuestionId)
                .ToList();

            if (!traitQuestions.Any())
            {
                personalityTestQuestionnaire.QuestionnaireTraits.Add(new QuestionnaireTrait
                {
                    Trait = trait.ToString(),
                    QuestionCount = 0,
                    Facets = new List<PersonalityTestFacet>()
                });
                
                continue;
            }

            var facetGroups = questionDetails
                .Where(qd => qd.HeadingId != null && traitQuestions.Contains(qd.Id))
                .GroupBy(qd => qd.HeadingId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var facets = new List<PersonalityTestFacet>();

            foreach (var facetHeading in facetHeadings)
            {
                if (!facetGroups.ContainsKey(facetHeading.Id))
                    continue;

                var questionsForFacet = facetGroups[facetHeading.Id]
                    .Select(q => new GetQuestionDetailsDto
                    {
                        QuestionId = q.Id,
                        Title = q.Title,
                        Type = q.QuestionType.ToString(),
                        Trait = trait.ToString(),
                        Heading = facetHeading.Title,
                        Rating = 0,
                        Answers = GetAnswersForQuestion(q)
                    })
                    .ToList();

                if (questionsForFacet.Any())
                {
                    facets.Add(new PersonalityTestFacet
                    {
                        Facet = facetHeading.Title,
                        Description = facetHeading.Description,
                        Questions = questionsForFacet
                    });
                }
            }

            personalityTestQuestionnaire.QuestionnaireTraits.Add(new QuestionnaireTrait
            {
                Trait = trait.ToString(),
                QuestionCount = facets.Sum(f => f.Questions.Count),
                Facets = facets
            });
        }

        personalityTestQuestionnaire.QuestionnaireTraits = isRandomizedDataRequired 
            ? personalityTestQuestionnaire.QuestionnaireTraits.OrderBy(_ => new Random().Next()).ToList()
            : personalityTestQuestionnaire.QuestionnaireTraits;
        
        return personalityTestQuestionnaire;
    }

    public GetPersonalityTestResponseDto GetPersonalityTestResponses(Guid userResponseId)
    {
        var userResponse = genericRepository.GetById<UserResponse>(userResponseId) 
                           ?? throw new NotFoundException("The following user response could not be found.");

        var questionnaire = genericRepository.GetById<Questionnaire>(userResponse.QuestionId) 
                            ?? throw new NotFoundException("The respective questionnaire could not be retrieved.");

        if (questionnaire.TrainingInspectionId == null)
            throw new NotFoundException("The respective questionnaire has not been assigned to a training.");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
            ?? throw new NotFoundException("The following questionnaire has not been assigned to a training.");

        var userResponseDetails = genericRepository.Get<UserResponseDetails>(x => 
            x.UserResponseId == userResponse.Id).ToList();
        
        var questionDetails = genericRepository.Get<QuestionnaireDetails>(x => 
            x.QuestionnaireId == questionnaire.Id).OrderBy(x => x.Order).ToList();

        var allQuestionnaireTraits = genericRepository.Get<QuestionnaireTraits>(x =>
            questionDetails.Select(z => z.Id).Contains(x.QuestionId)).ToList();

        var facetHeadings = genericRepository.Get<Heading>(x =>
            x.Facet == FacetType.Facet && 
            x.Type == HeadingType.Heading &&
            x.Inspection == InspectionType.PersonalityTest).ToList();

        var questionIds = questionDetails.Select(q => q.Id).ToList();
        var allAnswers = genericRepository.Get<Answer>(x => x.QuestionId != null &&
            questionIds.Contains(x.QuestionId.Value)).ToList();

        var personalityTestQuestionnaire = new List<QuestionnaireResponseTrait>();

        foreach (TraitType trait in Enum.GetValues(typeof(TraitType)))
        {
            var traitQuestions = allQuestionnaireTraits
                .Where(qt => qt.TraitType == trait)
                .Select(qt => qt.QuestionId)
                .ToList();

            if (!traitQuestions.Any())
            {
                personalityTestQuestionnaire.Add(new QuestionnaireResponseTrait
                {
                    Trait = trait.ToString(),
                    QuestionCount = 0,
                    Facets = new List<PersonalityTestResponseFacet>()
                });
                continue;
            }

            var facetGroups = questionDetails
                .Where(qd => qd.HeadingId != null && traitQuestions.Contains(qd.Id))
                .GroupBy(qd => qd.HeadingId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var facets = new List<PersonalityTestResponseFacet>();

            foreach (var facetHeading in facetHeadings)
            {
                if (!facetGroups.ContainsKey(facetHeading.Id))
                    continue;

                var questionsForFacet = new List<GetQuestionAnswerDetailsDto>();

                foreach (var questionDetail in facetGroups[facetHeading.Id])
                {
                    var answers = allAnswers
                        .Where(a => a.QuestionId == questionDetail.Id)
                        .OrderBy(x => x.Order)
                        .ToList();

                    var selectedAnswerIds = userResponseDetails
                        .Where(x => allAnswers.Any(a => a.Id == x.AnswerId && a.QuestionId == questionDetail.Id))
                        .Select(x => x.AnswerId)
                        .ToList();

                    var responseDetails = answers.Select(answer => new QuestionAnswerDetails
                    {
                        Id = answer.Id,
                        Title = answer.Title,
                        IsSelectable = true,
                        IsSelected = selectedAnswerIds.Contains(answer.Id),
                        Rating = 0
                    }).ToList();

                    questionsForFacet.Add(new GetQuestionAnswerDetailsDto
                    {
                        QuestionId = questionDetail.Id,
                        Heading = facetHeading.Title,
                        Title = questionDetail.Title,
                        QuestionType = questionDetail.QuestionType.ToString(),
                        Answers = responseDetails
                    });
                }

                if (questionsForFacet.Any())
                {
                    facets.Add(new PersonalityTestResponseFacet
                    {
                        Facet = facetHeading.Title,
                        Description = facetHeading.Description,
                        Questions = questionsForFacet
                    });
                }
            }

            personalityTestQuestionnaire.Add(new QuestionnaireResponseTrait
            {
                Trait = trait.ToString(),
                QuestionCount = facets.Sum(f => f.Questions.Count),
                Facets = facets
            });
        }

        return new GetPersonalityTestResponseDto
        {
            UserResponseId = userResponse.Id,
            QuestionnaireId = questionnaire.Id,
            TrainingInspectionId = trainingInspection.Id,
            Phase = userResponse.Phase,
            QuestionnaireTraits = personalityTestQuestionnaire
        };
    }
    
    public void UploadPersonalityTestAnswers(PersonalityTestRequestDto personalityTestAnswers)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId) 
                        ?? throw new NotFoundException("The following user has not been registered to our system.");
        
        var questionnaire = genericRepository.GetById<Questionnaire>(personalityTestAnswers.QuestionnaireId)
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

        if (phase == -1) throw new BadRequestException("Personality Tests could not be stored", ["No valid phase found for the current date."]);

        var userResponseDetails = new List<UserResponseDetails>();

        foreach (var answer in personalityTestAnswers.Answers)
        {
            var question = genericRepository.GetById<QuestionnaireDetails>(answer.QuestionId)
                ?? throw new NotFoundException("The following question identifier is not valid.");

            var answerModel = genericRepository.GetById<Answer>(answer.AnswerId)
                              ?? throw new NotFoundException("The following answer detail could not be found.");

            if (answerModel.QuestionId != question.Id)
                throw new NotFoundException("The following answer is not linked to the following questionnaire.");

            userResponseDetails.Add(new UserResponseDetails()
            {
                AnswerId = answerModel.Id,
            });
        }     
        
        var userResponse = new UserResponse()
        {
            QuestionId = questionnaire.Id,
            CandidateId = candidate.Id,
            AnsweredDate = DateTime.UtcNow,
            Remarks = "",
            IsAnsweredByCandidate = true,
            IsAnsweredBySubordinate = false,
            Phase = phase,
            UserResponseDetails = userResponseDetails
        };

        var userResponseId = genericRepository.Insert(userResponse);

        var analysis = CalculateResults(personalityTestAnswers.Answers);
    
        var score = new List<PersonalityTestScore>();
        
        var description = new List<PersonalityTestAnalysis>();
    
        foreach (var trait in analysis.Keys)
        {
            description.Add(new PersonalityTestAnalysis
            {
                Trait = trait.FromTraitType(),
                Description = GetTraitDescription(trait, analysis[trait]),
                Facets = GetFacetDescription(trait, analysis[trait])
            });

            score.Add(new PersonalityTestScore
            {
                Trait = trait.FromTraitType(),
                Score = (double) analysis[trait]
            });
        }

        var userResponseAnalysis = new UserResponseAnalysis()
        {
            UserResponseId = userResponseId,
            Title = "Personality Test - OCEAN Personality Traits",
            Description = keyValuePropertyService.SaveProperty("Analysis", description),
            Scores = keyValuePropertyService.SaveProperty("Scores", score)
        };

        genericRepository.Insert(userResponseAnalysis);
        
        var training = genericRepository.GetById<Training>(trainingInspection.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                                    x.TrainingId == training.Id && x.CandidateId == candidate.Id && x.IsApproved)
                                ?? throw new NotFoundException("The following candidate has not been assigned to the respective training.");
            
        certificationService.IssueTrainingCandidateCertification(new IssueCertificationDto()
        {
            TrainingCandidateId = trainingCandidate.Id
        });
    }

    public GetPersonalityTestAnalysisDto GetPersonalityTestAnalysis(Guid userResponseId)
    {
        var userResponse = genericRepository.GetById<UserResponse>(userResponseId)
                           ?? throw new NotFoundException("The following user response could not be found.");

        var userResponseAnalysis = genericRepository.GetFirstOrDefault<UserResponseAnalysis>(x => x.UserResponseId == userResponse.Id)
                                   ?? throw new NotFoundException("The following user response analysis could not be found.");

        var questionnaire = genericRepository.GetById<Questionnaire>(userResponse.QuestionId)
            ?? throw new NotFoundException("The following questionnaire could not be found.");

        if (questionnaire.TrainingInspectionId == null)
            throw new NotFoundException("The following questionnaire has not been linked to a questionnaire.");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException("The following inspection has not been assigned to any training.");
        
        var scores = keyValuePropertyService.GetProperty<List<PersonalityTestScore>>(userResponseAnalysis.Scores)
                     ?? throw new NotFoundException("The following user response scores could not be found.");

        var analyses = keyValuePropertyService.GetProperty<List<PersonalityTestAnalysis>>(userResponseAnalysis.Description)
                       ?? throw new NotFoundException("The following user response analysis could not be found.");

        return new GetPersonalityTestAnalysisDto()
        {
            UserResponseId = userResponse.Id,
            AnalysisId = userResponseAnalysis.Id,
            QuestionnaireId = questionnaire.Id,
            TrainingInspectionId = trainingInspection.Id,
            Scores = scores,
            Analyses = analyses
        };    
    }
    
    private Dictionary<TraitType, decimal> CalculateResults(List<PersonalityTestQuestionnaire> answers)
    {
        var result = new Dictionary<TraitType, decimal>();
        
        foreach (TraitType trait in Enum.GetValues(typeof(TraitType)))
        {
            result[trait] = 0.0m;
        }

        foreach (var answer in answers)
        {
            var question = genericRepository.GetById<QuestionnaireDetails>(answer.QuestionId)
                ?? throw new NotFoundException("The following questionnaire could not be found.");
            
            var answerModel = genericRepository.GetById<Answer>(answer.AnswerId)
                ?? throw new NotFoundException("The following answer could not be found.");

            if (answerModel.QuestionId != question.Id)
                throw new NotFoundException("The following answer is not linked to the following questionnaire.");

            var score = GetScoreViaAnswerTitle(answerModel.Title);

            var questionnaireTrait =
                genericRepository.GetFirstOrDefault<QuestionnaireTraits>(x => x.QuestionId == question.Id)
                ?? throw new NotFoundException("The following question does not have any traits linked to it.");
            
            result [questionnaireTrait.TraitType] += score;
        }

        foreach (var trait in result.Keys.ToList())
        {
            var questionCount = answers.Count(a =>
            {
                var question = genericRepository.GetById<QuestionnaireDetails>(a.QuestionId)
                               ?? throw new NotFoundException("The following questionnaire could not be found.");
                
                var questionnaireTrait =
                    genericRepository.GetFirstOrDefault<QuestionnaireTraits>(x => x.QuestionId == question.Id)
                    ?? throw new NotFoundException("The following question does not have any traits linked to it.");
                
                return questionnaireTrait.TraitType == trait;
            });

            result[trait] = questionCount == 0 ? 0 : result[trait] / questionCount;
        }

        return result;
    }

    private static string GetTraitDescription(TraitType trait, decimal score)
    {
        return trait switch
        {
            TraitType.Openness => score switch
            {
                >= 4 => "You’re likely imaginative, curious, and open to new experiences and ideas. You may thrive in roles that require creativity and adaptability, such as innovation, strategic planning, or product development.",
                >= 3 and < 4 => "You’re moderately open to new experiences but prefer a balance between creative and traditional approaches. You can handle some changes but may need clear structures and goals.",
                < 3 => "You prefer routine and traditional methods. You might be more comfortable in roles that emphasize consistency, accuracy, and following established procedures, like compliance, risk management, or auditing.",
            },
            
            TraitType.Conscientiousness => score switch
            {
                >= 4 => "You’re organized, disciplined, and responsible. You likely excel in management, project coordination, or roles requiring attention to detail, such as financial analysis or internal audit.",
                >= 3 and < 4 => "You balance responsibility with flexibility and perform well in structured settings but may occasionally need support for high-detail work.",
                < 3 => "You might struggle with organization and discipline and may need close supervision. You could be better suited for creative roles or dynamic environments where flexibility is valued over strict planning.",
            },
            
            TraitType.Extraversion => score switch
            {
                >= 4 => "You’re outgoing and energized by social interaction, likely performing well in client-facing roles, sales, or leadership positions.",
                >= 3 and < 4 => "You’re sociable when needed but also comfortable working independently, enjoying a mix of team-based and solo work.",
                < 3 => "You may prefer more reserved, back-office roles, data analysis, or research, focusing on independent work.",
            },
            
            TraitType.Agreeableness => score switch
            {
                >= 4 => "You’re cooperative and empathetic, likely excelling in team management, HR roles, or client relationship management.",
                >= 3 and < 4 => "You’re cooperative but can assert your ideas when needed, balancing teamwork with independent judgment.",
                < 3 => "You may focus more on task completion than on maintaining harmony, possibly fitting well into analytical roles where independence is key.",
            },
            
            TraitType.Neuroticism => score switch
            {
                >= 4 => "You may experience more anxiety or self-doubt. You might benefit from roles where predictability and support systems are in place, such as operations or administration.",
                >= 3 and < 4 => "You’re generally stable but may face occasional stress in challenging situations.",
                < 3 => "You’re calm and resilient, likely thriving in high-stress, fast-paced roles, like senior leadership, crisis management, or strategic decision-making.",
            },
            
            _ => string.Empty
        };
    }

    private static List<PersonalityTestAnalysis> GetFacetDescription(TraitType trait, decimal score)
    {
        var result = new List<PersonalityTestAnalysis>();

        switch (trait)
        {
            case TraitType.Openness:
                if (score > 3)
                {
                    result.Add(new PersonalityTestAnalysis { Trait = "Curiosity", Description = "You have a strong desire to explore new concepts and learn continuously." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Imagination", Description = "You are a visionary thinker." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Artistic Interests", Description = "You possess creativity even in non-artistic roles." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Adventurousness", Description = "You’re willing to take risks and try new strategies." });
                }
                else
                {
                    result.Add(new PersonalityTestAnalysis { Trait = "Curiosity", Description = "This indicates a preference for familiar tasks." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Imagination", Description = "It may reflect a preference for practical and concrete approaches." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Artistic Interests", Description = "You focus more on functionality than creativity." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Adventurousness", Description = "You may be cautious in unfamiliar situations." });
                }
                break;

            case TraitType.Conscientiousness:
                if (score > 3)
                {
                    result.Add(new PersonalityTestAnalysis { Trait = "Organization", Description = "You prefer structure and are well-organized." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Diligence", Description = "You have a strong work ethic and focus." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Self-discipline", Description = "You stay focused even in challenging situations." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Responsibility", Description = "You are dependable and take ownership." });
                }
                else
                {
                    result.Add(new PersonalityTestAnalysis { Trait = "Organization", Description = "You may thrive better in flexible environments." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Diligence", Description = "You may need external motivation to complete tasks." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Self-discipline", Description = "Maintaining concentration may be challenging for you." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Responsibility", Description = "You may require more guidance in task ownership." });
                }
                break;

            case TraitType.Extraversion:
                if (score > 3)
                {
                    result.Add(new PersonalityTestAnalysis { Trait = "Sociability", Description = "You enjoy social interactions and working with others." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Assertiveness", Description = "You have a natural inclination to lead and assert yourself." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Activity Level", Description = "You thrive in busy, fast-paced environments." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Enthusiasm", Description = "You approach work with excitement and positivity." });
                }
                else
                {
                    result.Add(new PersonalityTestAnalysis { Trait = "Sociability", Description = "You prefer independent work over social settings." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Assertiveness", Description = "You may hesitate to take charge in group settings." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Activity Level", Description = "You prefer a slower, more relaxed pace of work." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Enthusiasm", Description = "You approach work in a reserved, calm manner." });
                }
                break;

            case TraitType.Agreeableness:
                if (score > 3)
                {
                    result.Add(new PersonalityTestAnalysis { Trait = "Trust", Description = "You generally view others positively and are trusting." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Empathy", Description = "You are compassionate and attuned to others' feelings." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Cooperation", Description = "You have a collaborative nature and enjoy teamwork." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Modesty", Description = "You are humble about your achievements and contributions." });
                }
                else
                {
                    result.Add(new PersonalityTestAnalysis { Trait = "Trust", Description = "You may be more skeptical of others' intentions." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Empathy", Description = "You may be less attuned to others' emotions." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Cooperation", Description = "You may prefer working independently over collaborating." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Modesty", Description = "You are confident in asserting your achievements." });
                }
                break;

            case TraitType.Neuroticism:
                if (score > 3)
                {
                    result.Add(new PersonalityTestAnalysis { Trait = "Anxiety", Description = "You may frequently experience worry and anxiety." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Anger", Description = "You might be prone to frustration and irritability." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Sadness", Description = "You may experience discouragement more often than others." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Self-consciousness", Description = "You may be sensitive to feedback and self-critical." });
                }
                else
                {
                    result.Add(new PersonalityTestAnalysis { Trait = "Anxiety", Description = "You are generally calm and resilient under stress." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Anger", Description = "You remain composed even in challenging situations." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Sadness", Description = "You show resilience and optimism in the face of setbacks." });
                    result.Add(new PersonalityTestAnalysis { Trait = "Self-consciousness", Description = "You are confident in handling feedback and criticism." });
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(trait), trait, null);
        }

        return result;
    }

    
    private static decimal GetScoreViaAnswerTitle(string answer)
    {
        return answer switch
        {
            "Strongly Disagree" => 1,
            "Disagree" => 2,
            "Neutral" => 3,
            "Agree" => 4,
            "Strongly Agree" => 5,
            _ => 0
        };
    }
    
    private List<AnswerDetails> GetAnswersForQuestion(QuestionnaireDetails questionnaireDetail)
    {
        var questionDetailAnswers = genericRepository.Get<Answer>(x =>
            x.IsAnswerForQuestion && x.QuestionId == questionnaireDetail.Id).OrderBy(x => x.Order).ToList();

        var answers = questionDetailAnswers.Select(x => new AnswerDetails
        {
            Id = x.Id,
            Title = x.Title,
            IsSelectable = x.IsSelectable
        }).ToList();
        
        return answers;
    }
}