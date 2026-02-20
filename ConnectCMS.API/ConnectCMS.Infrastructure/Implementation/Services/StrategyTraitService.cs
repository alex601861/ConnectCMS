using CMSTrain.Helper;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Common.User;
using CMSTrain.Application.DTOs.Certification;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Strategy;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;
using TrainingInspectionConfigurationModule = CMSTrain.Domain.Common.Enum.Configurations.TrainingInspectionConfiguration;
using TrainingInspectionConfigurationModel = CMSTrain.Application.DTOs.Configuration.TrainingInspection.TrainingInspectionConfiguration;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class StrategyTraitService(IGenericRepository genericRepository, 
    ITrainingInspectionConfigurationService trainingInspectionConfigurationService,
    ICertificationService certificationService,
    ICurrentUserService userService) : IStrategyTraitService
{
    public List<GetStrategyDto> GetAllStrategies(StrategicType traitType, int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var strategicTraits = genericRepository.GetPagedResult<StrategicTrait>(pageNumber, pageSize, out rowCount, x => 
            x.Type == traitType && (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()))).ToList();

        var result = new List<GetStrategyDto>();

        foreach (var strategicTrait in strategicTraits)
        {
            var strategicDetails = genericRepository.Get<StrategicTraitDetails>(x => 
                x.TraitId == strategicTrait.Id).ToList();

            var strategicTraitDetails = genericRepository
                .Get<StrategicTrait>(x => 
                    strategicDetails.Select(z => z.DetailId).Contains(x.Id) && 
                    (x.Type != StrategicType.Strength || x.Type != StrategicType.Weakness)).ToList();
            
            var strategy = new GetStrategyDto()
            {
                Id = strategicTrait.Id,
                Name = strategicTrait.Name,
                Description = strategicTrait.Description,
                Type = strategicTrait.Type.ToString(),
                Opportunities = strategicTraitDetails.Where(x => x.Type == StrategicType.Opportunity).Select(x => new GetStrategyModuleDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Type = x.Type.ToString(),
                }).ToList(),
                Threats = strategicTraitDetails.Where(x => x.Type == StrategicType.Threat).Select(x => new GetStrategyModuleDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Type = x.Type.ToString(),
                }).ToList()
            };
            
            result.Add(strategy);
        }

        return result;
    }
    
    public List<GetStrategyDto> GetAllStrategies()
    {
        var strategicTraits = genericRepository.Get<StrategicTrait>(x => 
            x.Type == StrategicType.Strength || x.Type == StrategicType.Weakness).ToList();

        var result = new List<GetStrategyDto>();

        foreach (var strategicTrait in strategicTraits)
        {
            var strategicDetails = genericRepository.Get<StrategicTraitDetails>(x => 
                x.TraitId == strategicTrait.Id).ToList();

            var strategicTraitDetails = genericRepository
                .Get<StrategicTrait>(x => 
                    strategicDetails.Select(z => z.DetailId).Contains(x.Id) && 
                    (x.Type != StrategicType.Strength || x.Type != StrategicType.Weakness)).ToList();

            var strategy = new GetStrategyDto()
            {
                Id = strategicTrait.Id,
                Name = strategicTrait.Name,
                Description = strategicTrait.Description,
                Type = strategicTrait.Type.ToString(),
                Opportunities = strategicTraitDetails.Where(x => x.Type == StrategicType.Opportunity).Select(x => new GetStrategyModuleDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Type = x.Type.ToString(),
                }).ToList(),
                Threats = strategicTraitDetails.Where(x => x.Type == StrategicType.Threat).Select(x => new GetStrategyModuleDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Type = x.Type.ToString(),
                }).ToList()
            };
            
            result.Add(strategy);
        }

        return result;
    }

    public List<GetStrategyModuleDto> GetAllStrategyModules(StrategicType type)
    {
        switch (type)
        {
            case StrategicType.Strength:
                var strengths = genericRepository.Get<StrategicTrait>(x => x.Type == StrategicType.Strength).ToList();
                return strengths.Select(x => new GetStrategyModuleDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Type = x.Type.ToString(),
                }).ToList();
            case StrategicType.Weakness:
                var weaknesses = genericRepository.Get<StrategicTrait>(x => x.Type == StrategicType.Weakness).ToList();

                return weaknesses.Select(x => new GetStrategyModuleDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Type = x.Type.ToString(),
                }).ToList();
            default:
            case StrategicType.Threat:
            case StrategicType.Opportunity:
                throw new BadRequestException("The following requested trait type could not be found.", ["Only strength and weakness are allowed."]);
        }
    }

    public GetAllStrategyTraitResultsDto GetAllStrategyTraitResults(string? strengthIds, string? weaknessIds)
    {
        var strengthIdentifiers = strengthIds != null ? strengthIds.Split("$") : [];
        var weaknessIdentifiers = weaknessIds != null ? weaknessIds.Split("$") : [];

        var result = new GetAllStrategyTraitResultsDto()
        {
            Opportunities = [],
            Threats = []
        };
        
        foreach (var strengthId in strengthIdentifiers)
        {
            var strength = genericRepository.GetById<StrategicTrait>(Guid.Parse(strengthId))
                ?? throw new NotFoundException("The following strategic trait could not be found.");
            
            if (strength.Type != StrategicType.Strength)
                throw new BadRequestException("The following strategic trait could not mapped to its respective trait.", ["The following strategy is not a strength."]);

            var strengthTraitDetails = genericRepository.Get<StrategicTraitDetails>(x => 
                x.TraitId == strength.Id).ToList();
            
            var traitDetails = genericRepository.Get<StrategicTrait>(x => 
                strengthTraitDetails.Select(z => z.DetailId).Contains(x.Id)).ToList();
            
            result.Opportunities.AddRange(
                traitDetails
                    .Where(x => x.Type == StrategicType.Opportunity && result.Opportunities.All(o => o.Id != x.Id))
                    .Select(x => new GetStrategyModuleDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Type = x.Type.ToString(),
                    }).ToList());
            
            result.Threats.AddRange(
                traitDetails
                    .Where(x => x.Type == StrategicType.Threat && result.Threats.All(t => t.Id != x.Id))
                    .Select(x => new GetStrategyModuleDto()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Type = x.Type.ToString(),
                    }).ToList());
        }
        
        foreach (var weaknessId in weaknessIdentifiers)
        {
            var weakness = genericRepository.GetById<StrategicTrait>(Guid.Parse(weaknessId))
                             ?? throw new NotFoundException("The following strategic trait could not be found.");
            
            if (weakness.Type != StrategicType.Weakness)
                throw new BadRequestException("The following strategic trait could not mapped to its respective trait.", ["The following strategy is not a strength."]);

            var strengthTraitDetails = genericRepository.Get<StrategicTraitDetails>(x => 
                x.TraitId == weakness.Id).ToList();
            
            var traitDetails = genericRepository.Get<StrategicTrait>(x => 
                strengthTraitDetails.Select(z => z.DetailId).Contains(x.Id)).ToList();
            
            result.Opportunities.AddRange(
                traitDetails
                    .Where(x => x.Type == StrategicType.Opportunity && result.Opportunities.All(o => o.Id != x.Id))
                    .Select(x => new GetStrategyModuleDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Type = x.Type.ToString(),
                    }).ToList());
            
            result.Threats.AddRange(
                traitDetails
                    .Where(x => x.Type == StrategicType.Threat && result.Threats.All(t => t.Id != x.Id))
                    .Select(x => new GetStrategyModuleDto()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Type = x.Type.ToString(),
                    }).ToList());
        }

        return result;
    }
    
    public GetStrategyDto GetStrategyById(Guid strategyId)
    {
        var strategicTrait = genericRepository.GetById<StrategicTrait>(strategyId)
            ?? throw new NotFoundException("The following strategic trait could not be found.");
        
        var strategicDetails = genericRepository.Get<StrategicTraitDetails>(x => 
            x.TraitId == strategicTrait.Id).ToList();

        var strategicTraitDetails = genericRepository
            .Get<StrategicTrait>(x => 
                strategicDetails.Select(z => z.DetailId).Contains(x.Id) && 
                (x.Type != StrategicType.Strength || x.Type != StrategicType.Weakness)).ToList();

        return new GetStrategyDto()
        {
            Id = strategicTrait.Id,
            Name = strategicTrait.Name,
            Description = strategicTrait.Description,
            Type = strategicTrait.Type.ToString(),
            Opportunities = strategicTraitDetails.Where(x => x.Type == StrategicType.Opportunity).Select(x => new GetStrategyModuleDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Type = x.Type.ToString(),
            }).ToList(),
            Threats = strategicTraitDetails.Where(x => x.Type == StrategicType.Threat).Select(x => new GetStrategyModuleDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Type = x.Type.ToString(),
            }).ToList()
        };
    }

    public GetStrategyDetailsDto GetStrategyDetails()
    {
        var opportunities = genericRepository.Get<StrategicTrait>(x => x.Type == StrategicType.Opportunity).ToList();
        
        var threats = genericRepository.Get<StrategicTrait>(x => x.Type == StrategicType.Threat).ToList();

        return new GetStrategyDetailsDto()
        {
            Opportunities = opportunities.Select(x => new GetStrategyModuleDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Type = x.Type.ToString(),
            }).ToList(),
            Threats = threats.Select(x => new GetStrategyModuleDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Type = x.Type.ToString(),
            }).ToList(),
        };
    }

    public void InsertStrategy(InsertStrategyDto strategy)
    {
        var strategyModel = new StrategicTrait()
        {
            Name = strategy.Name,
            Description = strategy.Description,
            Type = strategy.Type
        };

        genericRepository.Insert(strategyModel);
    }

    public void UpdateStrategy(UpdateStrategyDto strategy)
    {
        var strategyModel = genericRepository.GetById<StrategicTrait>(strategy.Id)
                            ?? throw new NotFoundException("The following strategic trait could not be found.");

        strategyModel.Name = strategy.Name;
        strategyModel.Description = strategy.Description;
        
        genericRepository.Update(strategyModel);

        if (strategyModel.Type == strategy.Type) return;
        
        var strategicTraitDetails =
            genericRepository.Get<StrategicTraitDetails>(x =>
                x.TraitId == strategyModel.Id || x.DetailId == strategyModel.Id);
        
        if (strategicTraitDetails.Any())
            throw new PartialException("The following strategic trait's type could not be updated. The following strategy is already linked to an opportunity or a threat.");
        
        strategyModel.Type = strategy.Type;
        
        genericRepository.Update(strategyModel);
    }

    public void DeleteStrategy(Guid strategyId)
    {
        var strategyModel = genericRepository.GetById<StrategicTrait>(strategyId)
                            ?? throw new NotFoundException("The following strategic trait could not be found.");

        var strategicTraitDetails =
            genericRepository.Get<StrategicTraitDetails>(x =>
                x.TraitId == strategyModel.Id || x.DetailId == strategyModel.Id).ToList();

        if (strategicTraitDetails.Any())
        {
            genericRepository.RemoveMultipleEntity(strategicTraitDetails);
        }
        
        var strategicTraitResponses =
            genericRepository.Get<StrategicTraitResponseDetails>(x =>
                x.StrategicTraitId == strategyModel.Id).ToList();

        if (strategicTraitResponses.Any())
        {
            genericRepository.RemoveMultipleEntity(strategicTraitResponses);
        }
        
        genericRepository.Delete(strategyModel);
    }

    public void UploadStrategyDetails(UploadStrategyDetailsDto strategyDetails)
    {
        var strategyModel = genericRepository.GetById<StrategicTrait>(strategyDetails.StrategyId)
                            ?? throw new NotFoundException("The following strategic trait could not be found.");
        
        if (strategyModel.Type is not (StrategicType.Strength or StrategicType.Weakness))
            throw new BadRequestException("The following strategic trait cannot be uploaded.", ["The following strategy is neither a strength nor weakness."]);

        var strategicTraitDetails =
            genericRepository.Get<StrategicTraitDetails>(x => x.TraitId == strategyModel.Id).ToList();
        
        if (strategicTraitDetails.Count != 0)
           genericRepository.RemoveMultipleEntity(strategicTraitDetails);

        foreach (var opportunityId in strategyDetails.Opportunities)
        {
            var opportunity = genericRepository.GetById<StrategicTrait>(opportunityId)
                              ?? throw new NotFoundException("The following strategic trait could not be found.");

            if (opportunity.Type is not StrategicType.Opportunity)
                throw new BadRequestException("The following strategic trait cannot be uploaded.",
                    ["The selected strategy trait does not belong to the opportunity model."]);

            var opportunityModel = new StrategicTraitDetails()
            {
                TraitId = strategyModel.Id,
                DetailId = opportunity.Id
            };
            
            genericRepository.Insert(opportunityModel);
        }
        
        foreach (var threatId in strategyDetails.Threats)
        {
            var threat = genericRepository.GetById<StrategicTrait>(threatId)
                         ?? throw new NotFoundException("The following strategic trait could not be found.");

            if (threat.Type is not StrategicType.Threat)
                throw new BadRequestException("The following strategic trait cannot be uploaded.",
                    ["The selected strategy trait does not belong to the threat model."]);

            var threatModel = new StrategicTraitDetails()
            {
                TraitId = strategyModel.Id,
                DetailId = threat.Id
            };
            
            genericRepository.Insert(threatModel);
        }
    }

    public void UploadStrategyTraitQuestionnaire(UploadStrategyTraitQuestionnaireDto strategyDetails)
    {
        var questionnaire = genericRepository.GetById<Questionnaire>(strategyDetails.QuestionnaireId)
            ?? throw new NotFoundException("The following questionnaire could not be found.");

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

        if (phase == -1) throw new BadRequestException("SWOT Analysis could not be performed", ["No valid phase found for the current date."]);
        
        var strengths = strategyDetails.StrengthIds.Count;
        var weaknesses = strategyDetails.WeaknessIds.Count;
        
        var opportunities = new HashSet<Guid>();
        var threats = new HashSet<Guid>();

        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException("The following user has not been registered to our system.");

        var strengthDetails = new List<StrategicTrait>();
        var weaknessDetails = new List<StrategicTrait>();

        foreach (var strengthId in strategyDetails.StrengthIds)
        {
            ProcessTraitDetails(strengthId, StrategicType.Strength, strengthDetails);
        }

        foreach (var weaknessId in strategyDetails.WeaknessIds)
        {
            ProcessTraitDetails(weaknessId, StrategicType.Weakness, weaknessDetails);
        }

        var combinedDetails = strengthDetails.Concat(weaknessDetails).ToList();

        var strategyTraitResponse = new StrategicTraitResponse()
        {
            QuestionnaireId = questionnaire.Id,
            Phase = phase,
            CandidateId = candidate.Id,
            Strengths = strengths,
            Weaknesses = weaknesses,
            Opportunities = opportunities.Count,
            Threats = threats.Count,
            StrategicTraitResponseDetails = combinedDetails.Select(x => new StrategicTraitResponseDetails()
            {
                StrategicTraitId = x.Id
            }).ToList()
        };

        genericRepository.Insert(strategyTraitResponse);
        
        var training = genericRepository.GetById<Training>(trainingInspection.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                                    x.TrainingId == training.Id && x.CandidateId == candidate.Id && x.IsApproved)
                                ?? throw new NotFoundException("The following candidate has not been assigned to the respective training.");
            
        certificationService.IssueTrainingCandidateCertification(new IssueCertificationDto()
        {
            TrainingCandidateId = trainingCandidate.Id
        });
        
        return;

        void ProcessTraitDetails(Guid traitId, StrategicType expectedType, List<StrategicTrait> traitDetails)
        {
            var strategyModel = genericRepository.GetById<StrategicTrait>(traitId)
                                ?? throw new NotFoundException("The following strategic trait could not be found.");

            if (strategyModel.Type != expectedType)
                throw new BadRequestException("The following strategic trait cannot be uploaded.", new[] { $"The following strategy is not a {expectedType.ToString().ToLower()}." });

            traitDetails.Add(strategyModel);

            var strategicTraitDetails = genericRepository.Get<StrategicTraitDetails>(x => x.TraitId == strategyModel.Id).ToList();
            var traitIds = strategicTraitDetails.Select(x => x.DetailId).ToList();

            opportunities.UnionWith(genericRepository.Get<StrategicTrait>(x => 
                    x.Type == StrategicType.Opportunity && traitIds.Contains(x.Id)).Select(x => x.Id).ToList());
            threats.UnionWith(genericRepository.Get<StrategicTrait>(x => 
                    x.Type == StrategicType.Threat && traitIds.Contains(x.Id)).Select(x => x.Id).ToList());
        }
    }

    public GetStrategicTraitCountDto GetStrategicTraitCount()
    {
        var strengths = genericRepository.GetCount<StrategicTrait>(x => x.Type == StrategicType.Strength);
        var weaknesses = genericRepository.GetCount<StrategicTrait>(x => x.Type == StrategicType.Weakness);
        var opportunities = genericRepository.GetCount<StrategicTrait>(x => x.Type == StrategicType.Opportunity);
        var threats = genericRepository.GetCount<StrategicTrait>(x => x.Type == StrategicType.Threat);

        return new GetStrategicTraitCountDto()
        {
            Strengths = strengths,
            Weaknesses = weaknesses,
            Opportunities = opportunities,
            Threats = threats
        };
    }
    
    public List<GetStrategyTraitQuestionnaireDto> GetStrategyTraitQuestionnaireResponses(int pageNumber, int pageSize, out int rowCount, DateTime? startDate = null, DateTime? endDate = null)
    {
        var userId = userService.GetUserId;

        var user = genericRepository.GetById<User>(userId)
                        ?? throw new NotFoundException("The following user has not been registered to our system.");
        
        var strategyTraitResponses = genericRepository.GetPagedResult<StrategicTraitResponse>(
            pageNumber, pageSize, out rowCount, 
            x => x.CandidateId == user.Id && 
                 (startDate == null || endDate == null || 
                  (x.CreatedAt >= startDate.Value.ToUniversalTime() && x.CreatedAt <= endDate.Value.ToUniversalTime()))
        ).ToList();

        return strategyTraitResponses.Select(x => new GetStrategyTraitQuestionnaireDto()
        {
            Id = x.Id,
            Strengths = x.Strengths,
            Opportunities = x.Opportunities,
            Weaknesses = x.Weaknesses,
            Threats = x.Threats,
            AnsweredDate = x.CreatedAt.ToFormattedDateTime()
        }).ToList();
    }
    
    public List<GetStrategyTraitQuestionnaireDto> GetStrategyTraitQuestionnaireResponses(DateTime? startDate = null, DateTime? endDate = null)
    {
        var userId = userService.GetUserId;

        var user = genericRepository.GetById<User>(userId)
                        ?? throw new NotFoundException("The following user has not been registered to our system.");
        
        var strategyTraitResponses = genericRepository.Get<StrategicTraitResponse>(x => x.CandidateId == user.Id && 
                 (startDate == null || endDate == null || 
                  (x.CreatedAt >= startDate.Value.ToUniversalTime() && x.CreatedAt <= endDate.Value.ToUniversalTime()))).ToList();

        return strategyTraitResponses.Select(x => new GetStrategyTraitQuestionnaireDto()
        {
            Id = x.Id,
            Strengths = x.Strengths,
            Opportunities = x.Opportunities,
            Weaknesses = x.Weaknesses,
            Threats = x.Threats,
            AnsweredDate = x.CreatedAt.ToFormattedDateTime()
        }).ToList();
    }

    public List<GetStrategyTraitQuestionnaireDto> GetStrategyTraitQuestionnaireResponsesByUserId(Guid userId, int pageNumber, int pageSize, out int rowCount)
    {
        var user = genericRepository.GetById<User>(userId)
                   ?? throw new NotFoundException("The following user has not been registered to our system.");
        
        var strategyTraitResponses = genericRepository.GetPagedResult<StrategicTraitResponse>(pageNumber, pageSize, out rowCount, x => x.CandidateId == user.Id).ToList();

        return strategyTraitResponses.Select(x => new GetStrategyTraitQuestionnaireDto()
        {
            Id = x.Id,
            Strengths = x.Strengths,
            Opportunities = x.Opportunities,
            Weaknesses = x.Weaknesses,
            Threats = x.Threats,
            AnsweredDate = x.CreatedAt.ToFormattedDateTime()
        }).ToList();
    }

    public List<GetStrategyTraitQuestionnaireDto> GetStrategyTraitQuestionnaireResponsesByUserId(Guid userId)
    {
        var user = genericRepository.GetById<User>(userId)
                   ?? throw new NotFoundException("The following user has not been registered to our system.");
        
        var strategyTraitResponses = genericRepository.Get<StrategicTraitResponse>(x => x.CandidateId == user.Id).ToList();

        return strategyTraitResponses.Select(x => new GetStrategyTraitQuestionnaireDto()
        {
            Id = x.Id,
            Strengths = x.Strengths,
            Opportunities = x.Opportunities,
            Weaknesses = x.Weaknesses,
            Threats = x.Threats,
            AnsweredDate = x.CreatedAt.ToFormattedDateTime()
        }).ToList();
    }
    
    public GetStrategyTraitQuestionnaireDetailsDto GetStrategyTraitQuestionnaireDetails(Guid responseId)
    {
        var strategyTraitResponse = genericRepository.GetById<StrategicTraitResponse>(responseId)
            ?? throw new NotFoundException("The following user response for strategic traits has not been answered.");

        var questionnaire = genericRepository.GetById<Questionnaire>(strategyTraitResponse.QuestionnaireId) 
                            ?? throw new NotFoundException("The following questionnaire for strategic traits could not be found.");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId) 
            ?? throw new NotFoundException("The following training inspection could not be found.");

        var strategicTraitResponseDetails = genericRepository.Get<StrategicTraitResponseDetails>(x => 
            x.StrategicTraitResponseId == strategyTraitResponse.Id).ToList();

        var strategicTraits = genericRepository.Get<StrategicTrait>(x => 
            x.Type == StrategicType.Strength || x.Type == StrategicType.Weakness).ToList();

        var strategicTraitDetails = genericRepository.Get<StrategicTraitDetails>(x => 
            strategicTraits.Select(t => t.Id).Contains(x.TraitId)).ToList();

        var relatedTraits = genericRepository.Get<StrategicTrait>(x =>
            strategicTraitDetails.Select(d => d.DetailId).Contains(x.Id)).ToDictionary(x => x.Id);

        var strengths = new List<Traits>();
        var weaknesses = new List<Traits>();
        var opportunities = new List<GetStrategyModuleDto>();
        var threats = new List<GetStrategyModuleDto>();

        var result = new GetStrategyTraitQuestionnaireDetailsDto
        {
            Questionnaire = new GetStrategyTraitQuestionnaireDto
            {
                Id = strategyTraitResponse.Id, 
                Strengths = strategyTraitResponse.Strengths,
                Opportunities = strategyTraitResponse.Opportunities,
                Weaknesses = strategyTraitResponse.Weaknesses,
                Threats = strategyTraitResponse.Threats,
                AnsweredDate = strategyTraitResponse.CreatedAt.ToFormattedDateTime(),
            },
            Strengths = strengths,
            Weaknesses = weaknesses,
            Opportunities = opportunities,
            Threats = threats,
            QuestionnaireId = strategyTraitResponse.QuestionnaireId,
            TrainingId = trainingInspection.TrainingId
        };

        foreach (var strategicTrait in strategicTraits)
        {
            var isSelected = strategicTraitResponseDetails.Any(x => x.StrategicTraitId == strategicTrait.Id);
            var traitDto = new Traits
            {
                Id = strategicTrait.Id,
                Name = strategicTrait.Name,
                Description = strategicTrait.Description,
                Type = strategicTrait.Type.ToString(),
                IsSelected = isSelected
            };

            switch (strategicTrait.Type)
            {
                case StrategicType.Strength:
                    strengths.Add(traitDto);
                    break;
                case StrategicType.Weakness:
                    weaknesses.Add(traitDto);
                    break;
            }

            if (!isSelected) continue;
            {
                var relatedTraitDetails = strategicTraitDetails.Where(d => d.TraitId == strategicTrait.Id).ToList();

                foreach (var detail in relatedTraitDetails)
                {
                    if (relatedTraits.TryGetValue(detail.DetailId, out var relatedTrait))
                    {
                        switch (relatedTrait.Type)
                        {
                            case StrategicType.Opportunity when opportunities.All(x => x.Id != relatedTrait.Id):
                                opportunities.Add(new GetStrategyModuleDto
                                {
                                    Id = relatedTrait.Id,
                                    Name = relatedTrait.Name,
                                    Description = relatedTrait.Description,
                                    Type = StrategicType.Opportunity.ToString(),
                                });
                                break;
                            case StrategicType.Threat when threats.All(x => x.Id != relatedTrait.Id):
                                threats.Add(new GetStrategyModuleDto
                                {
                                    Id = relatedTrait.Id,
                                    Name = relatedTrait.Name,
                                    Description = relatedTrait.Description,
                                    Type = StrategicType.Threat.ToString(),
                                });
                                break;
                        }
                    }
                }
            }
        }

        return result;
    }
}