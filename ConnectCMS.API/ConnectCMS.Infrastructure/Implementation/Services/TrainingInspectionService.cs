using CMSTrain.Helper;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using Microsoft.Extensions.Options;
using CMSTrain.Application.Settings;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.DTOs.Email;
using CMSTrain.Application.Common.User;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.TrainingInspection;
using CMSTrain.Application.Interfaces.Repositories.Base;
using CMSTrain.Application.DTOs.Configuration.TrainingInspection;
using TrainingInspectionConfiguration = CMSTrain.Domain.Entities.TrainingInspectionConfiguration;
using TrainingInspectionConfigurationModule = CMSTrain.Domain.Common.Enum.Configurations.TrainingInspectionConfiguration;
using TrainingInspectionConfigurationModel = CMSTrain.Application.DTOs.Configuration.TrainingInspection.TrainingInspectionConfiguration;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class TrainingInspectionService(IOptions<ClientSettings> clientSettings,
    IEmailService emailService,
    ICurrentUserService userService, 
    IHangfireService hangfireService,
    IGenericRepository genericRepository, 
    ITrainingInspectionConfigurationService trainingInspectionConfigurationService) : ITrainingInspectionService
{
    private readonly string _baseUrl = clientSettings.Value.BaseUrl.Split(";").FirstOrDefault() 
                                       ?? throw new NotFoundException("The Base URL has not been stabilized and initialized");
    public GetTrainingInspectionDetailsDto GetTrainingInspectionById(Guid trainingInspectionId)
    {
        var trainingInspection = genericRepository.GetById<TrainingInspection>(trainingInspectionId)
                                  ?? throw new NotFoundException("The respective inspection has not been assigned to the following training.");
        
        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The respective inspection could not be found.");
        
        var training = genericRepository.GetById<Training>(trainingInspection.TrainingId)
                         ?? throw new NotFoundException("The respective training could not be found.");

        var inspectionConfiguration =
            trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());
        
        var questionnaire = genericRepository.GetFirstOrDefault<Questionnaire>(x =>
            x.IsQuestionnaireForTraining && x.TrainingInspectionId == trainingInspection.Id);
        
        return new GetTrainingInspectionDetailsDto()
        {
            TrainingInspectionId = trainingInspection.Id,
            InspectionId = inspection.Id,
            TrainingId = training.Id,
            QuestionnaireId = questionnaire?.Id ?? Guid.Empty,
            Phases = inspectionConfiguration?.Accessibility.Count ?? 0
        };
    }

    public GetTrainingInspectionDetailsDto GetTrainingInspectionByQuestionnaire(Guid questionnaireId)
    {
        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
                            ?? throw new NotFoundException("The following questionnaire could not be found.");
        
        if (questionnaire.TrainingInspectionId == null) throw new NotFoundException("The following questionnaire has not been assigned to any training inspection.");
     
        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException("The respective inspection has not been assigned to the following training.");
        
        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The respective inspection could not be found.");
        
        var training = genericRepository.GetById<Training>(trainingInspection.TrainingId)
                       ?? throw new NotFoundException("The respective training could not be found.");

        var inspectionConfiguration =
            trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());
        
        return new GetTrainingInspectionDetailsDto()
        {
            TrainingInspectionId = trainingInspection.Id,
            InspectionId = inspection.Id,
            TrainingId = training.Id,
            QuestionnaireId = questionnaire.Id,
            Phases = inspectionConfiguration?.Accessibility.Count ?? 0
        };
    }
    
    public List<GetTrainingInspectionDto> GetAllAssignedTrainingInspections(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId)
            ?? throw new NotFoundException("The respective training could not be found.");   

        var trainingInspections = genericRepository.GetPagedResult<TrainingInspection>(pageNumber, pageSize, out rowCount, x => x.TrainingId == training.Id).ToList();

        var result = new List<GetTrainingInspectionDto>();
        
        foreach (var trainingInspection in trainingInspections)
        {
            var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId) 
                             ?? throw new NotFoundException("The respective inspection could not be found.");

            if (!string.IsNullOrEmpty(search) && !inspection.Name.ToLower().Contains(search.ToLower()))
                continue;
            
            var questionnaire =
                genericRepository.GetFirstOrDefault<Questionnaire>(x =>
                    x.TrainingInspectionId == trainingInspection.Id && x.IsQuestionnaireForTraining);

            var inspectionQuestionnaire =
                genericRepository.GetCount<InspectionQuestionnaires>(x => x.InspectionId == inspection.Id);

            var inspectionConfiguration =
                trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                    trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());
            
            result.Add(new GetTrainingInspectionDto()
            {
                Id = inspection.Id,
                Name = inspection.Name,
                Description = inspection.Description,
                ImageUrl = inspection.ImageUrl,
                HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis || inspectionQuestionnaire != 0,
                PhasesCount = inspectionConfiguration?.Accessibility.Count ?? 0,
                IsActive = inspection.IsActive,
                Type = inspection.InspectionType.ToInspectionType(),
                TrainingInspectionId = trainingInspection.Id,
                QuestionnaireId = questionnaire?.Id,
                IsQuestionnaireUploaded = questionnaire != null,
                UploadedDate = questionnaire?.CreatedAt.ToFormattedDateTime() ?? "",
            });
        }

        return result;
    }

    public List<GetTrainingInspectionDto> GetAllAssignedTrainingInspections(Guid trainingId, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId)
            ?? throw new NotFoundException("The respective training could not be found.");
    
        var trainingInspections = genericRepository.Get<TrainingInspection>(x => x.TrainingId == training.Id).ToList();
    
        var result = new List<GetTrainingInspectionDto>();
    
        foreach (var trainingInspection in trainingInspections)
        {
            var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId) 
                             ?? throw new NotFoundException("The respective inspection could not be found.");
    
            if (!string.IsNullOrEmpty(search) && !inspection.Name.ToLower().Contains(search.ToLower()))
                continue;
            
            var questionnaire =
                genericRepository.GetFirstOrDefault<Questionnaire>(x =>
                    x.TrainingInspectionId == trainingInspection.Id);

            var inspectionQuestionnaire =
                genericRepository.GetCount<InspectionQuestionnaires>(x => x.InspectionId == inspection.Id);
            
            var inspectionConfiguration =
                trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                    trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());
            
            result.Add(new GetTrainingInspectionDto()
            {
                Id = inspection.Id,
                Name = inspection.Name,
                Description = inspection.Description,
                ImageUrl = inspection.ImageUrl,
                HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis || inspectionQuestionnaire != 0,
                PhasesCount = inspectionConfiguration?.Accessibility.Count ?? 0,
                IsActive = inspection.IsActive,
                Type = inspection.InspectionType.ToInspectionType(),
                TrainingInspectionId = trainingInspection.Id,
                QuestionnaireId = questionnaire?.Id,
                IsQuestionnaireUploaded = questionnaire != null,
                UploadedDate = questionnaire?.CreatedAt.ToFormattedDateTime() ?? "",
            });
        }
    
        return result;
    }

    public List<GetTrainingInspectionDto> GetAllAssignedTrainingInspectionsForCandidateAndClient(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId)
            ?? throw new NotFoundException("The respective training could not be found.");   

        var trainingInspections = genericRepository.GetPagedResult<TrainingInspection>(pageNumber, pageSize, out rowCount, x => x.TrainingId == training.Id).ToList();

        var result = new List<GetTrainingInspectionDto>();
        
        foreach (var trainingInspection in trainingInspections)
        {
            var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId) 
                             ?? throw new NotFoundException("The respective inspection could not be found.");

            if (!string.IsNullOrEmpty(search) && !inspection.Name.ToLower().Contains(search.ToLower()))
                continue;
            
            var questionnaire =
                genericRepository.GetFirstOrDefault<Questionnaire>(x =>
                    x.TrainingInspectionId == trainingInspection.Id && x.IsQuestionnaireForTraining);

            if (questionnaire == null) continue;
            
            var inspectionQuestionnaire =
                genericRepository.GetCount<InspectionQuestionnaires>(x => x.InspectionId == inspection.Id);

            var inspectionConfiguration =
                trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                    trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());
            
            result.Add(new GetTrainingInspectionDto()
            {
                Id = inspection.Id,
                Name = inspection.Name,
                Description = inspection.Description,
                ImageUrl = inspection.ImageUrl,
                HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis || inspectionQuestionnaire != 0,
                PhasesCount = inspectionConfiguration?.Accessibility.Count ?? 0,
                IsActive = inspection.IsActive,
                Type = inspection.InspectionType.ToInspectionType(),
                TrainingInspectionId = trainingInspection.Id,
                QuestionnaireId = questionnaire.Id,
                IsQuestionnaireUploaded = true,
                UploadedDate = questionnaire.CreatedAt.ToFormattedDateTime(),
            });
        }

        return result;
    }

    public List<GetTrainingInspectionDto> GetAllAssignedTrainingInspectionsForCandidateAndClient(Guid trainingId, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId)
            ?? throw new NotFoundException("The respective training could not be found.");
    
        var trainingInspections = genericRepository.Get<TrainingInspection>(x => x.TrainingId == training.Id).ToList();
    
        var result = new List<GetTrainingInspectionDto>();
    
        foreach (var trainingInspection in trainingInspections)
        {
            var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId) 
                             ?? throw new NotFoundException("The respective inspection could not be found.");
    
            if (!string.IsNullOrEmpty(search) && !inspection.Name.ToLower().Contains(search.ToLower()))
                continue;
            
            var questionnaire =
                genericRepository.GetFirstOrDefault<Questionnaire>(x =>
                    x.TrainingInspectionId == trainingInspection.Id);

            if (questionnaire == null) continue;

            var inspectionQuestionnaire =
                genericRepository.GetCount<InspectionQuestionnaires>(x => x.InspectionId == inspection.Id);
            
            var inspectionConfiguration =
                trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                    trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());
            
            result.Add(new GetTrainingInspectionDto()
            {
                Id = inspection.Id,
                Name = inspection.Name,
                Description = inspection.Description,
                ImageUrl = inspection.ImageUrl,
                HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis || inspectionQuestionnaire != 0,
                PhasesCount = inspectionConfiguration?.Accessibility.Count ?? 0,
                IsActive = inspection.IsActive,
                Type = inspection.InspectionType.ToInspectionType(),
                TrainingInspectionId = trainingInspection.Id,
                QuestionnaireId = questionnaire.Id,
                IsQuestionnaireUploaded = true,
                UploadedDate = questionnaire.CreatedAt.ToFormattedDateTime(),
            });
        }
    
        return result;
    }

    public List<GetTrainingInspectionDto> GetAllAssignedTrainingInspectionsForTrainingCandidate(Guid trainingCandidateId, int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
            ?? throw new NotFoundException("The following candidate has not been assigned to the respective training.");
        
        var training = genericRepository.GetById<Training>(trainingCandidate.TrainingId)
                       ?? throw new NotFoundException("The respective training could not be found.");   

        var trainingInspections = genericRepository.GetPagedResult<TrainingInspection>(pageNumber, pageSize, out rowCount, x => x.TrainingId == training.Id).ToList();

        var result = new List<GetTrainingInspectionDto>();
        
        foreach (var trainingInspection in trainingInspections)
        {
            var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId) 
                             ?? throw new NotFoundException("The respective inspection could not be found.");

            if (!string.IsNullOrEmpty(search) && !inspection.Name.ToLower().Contains(search.ToLower()))
                continue;
            
            var questionnaire =
                genericRepository.GetFirstOrDefault<Questionnaire>(x =>
                    x.TrainingInspectionId == trainingInspection.Id && x.IsQuestionnaireForTraining);

            if (questionnaire == null) continue;
            
            var inspectionQuestionnaire =
                genericRepository.GetCount<InspectionQuestionnaires>(x => x.InspectionId == inspection.Id);

            var inspectionConfiguration =
                trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                    trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());
            
            result.Add(new GetTrainingInspectionDto()
            {
                Id = inspection.Id,
                Name = inspection.Name,
                Description = inspection.Description,
                ImageUrl = inspection.ImageUrl,
                HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis || inspectionQuestionnaire != 0,
                PhasesCount = inspectionConfiguration?.Accessibility.Count ?? 0,
                IsActive = inspection.IsActive,
                Type = inspection.InspectionType.ToInspectionType(),
                TrainingInspectionId = trainingInspection.Id,
                QuestionnaireId = questionnaire.Id,
                IsQuestionnaireUploaded = true,
                UploadedDate = questionnaire.CreatedAt.ToFormattedDateTime(),
            });
        }

        return result;
    }

    public List<GetTrainingInspectionDto> GetAllAssignedTrainingInspectionsForTrainingCandidate(Guid trainingCandidateId, string? search = null)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
            ?? throw new NotFoundException("The following candidate has not been assigned to the respective training.");
        
        var training = genericRepository.GetById<Training>(trainingCandidate.TrainingId)
                       ?? throw new NotFoundException("The respective training could not be found.");   

        var trainingInspections = genericRepository.Get<TrainingInspection>(x => x.TrainingId == training.Id).ToList();

        var result = new List<GetTrainingInspectionDto>();
        
        foreach (var trainingInspection in trainingInspections)
        {
            var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId) 
                             ?? throw new NotFoundException("The respective inspection could not be found.");

            if (!string.IsNullOrEmpty(search) && !inspection.Name.ToLower().Contains(search.ToLower()))
                continue;
            
            var questionnaire =
                genericRepository.GetFirstOrDefault<Questionnaire>(x =>
                    x.TrainingInspectionId == trainingInspection.Id && x.IsQuestionnaireForTraining);

            if (questionnaire == null) continue;
            
            var inspectionQuestionnaire =
                genericRepository.GetCount<InspectionQuestionnaires>(x => x.InspectionId == inspection.Id);

            var inspectionConfiguration =
                trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                    trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());
            
            result.Add(new GetTrainingInspectionDto()
            {
                Id = inspection.Id,
                Name = inspection.Name,
                Description = inspection.Description,
                ImageUrl = inspection.ImageUrl,
                HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis || inspectionQuestionnaire != 0,
                PhasesCount = inspectionConfiguration?.Accessibility.Count ?? 0,
                IsActive = inspection.IsActive,
                Type = inspection.InspectionType.ToInspectionType(),
                TrainingInspectionId = trainingInspection.Id,
                QuestionnaireId = questionnaire.Id,
                IsQuestionnaireUploaded = true,
                UploadedDate = questionnaire.CreatedAt.ToFormattedDateTime(),
            });
        }

        return result;
    }
    
    public GetCandidateTrainingInspectionDto GetCandidateTrainingInspectionDetails(Guid trainingInspectionId)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");
        
        var trainingInspection = genericRepository.GetById<TrainingInspection>(trainingInspectionId)
                                 ?? throw new NotFoundException("The respective inspection has not been assigned to the following training.");

        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The following inspection could not be found.");

        var result = new GetCandidateTrainingInspectionDto()
        {
            Id = inspection.Id,
            Name = inspection.Name,
            Description = inspection.Description,
            ImageUrl = inspection.ImageUrl,
            Type = inspection.InspectionType.ToInspectionType(),
            IsActive = inspection.IsActive,
            HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis ||
                                       genericRepository.GetCount<InspectionQuestionnaires>(z =>
                                           z.InspectionId == inspection.Id) != 0,
        };
        
        var trainingInspectionConfiguration = 
            trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());

        if (trainingInspectionConfiguration == null) return result;

        result.PhasesCount = trainingInspectionConfiguration.Accessibility.Count;
        
        var questionnaire = genericRepository.GetFirstOrDefault<Questionnaire>(x =>
                                x.TrainingInspectionId == trainingInspection.Id && x.IsQuestionnaireForTraining)
                            ?? throw new NotFoundException("The following questionnaire could not be found.");
            
        foreach (var accessibility in trainingInspectionConfiguration.Accessibility)
        {
            Guid? userResponseId;
            DateTime? userResponseDate;
            
            if (inspection.InspectionType != InspectionType.SwotAnalysis)
            {
                var userResponseModel = genericRepository.GetFirstOrDefault<UserResponse>(x =>
                    x.QuestionId == questionnaire.Id && x.CandidateId == candidate.Id && x.IsAnsweredByCandidate && x.Phase == trainingInspectionConfiguration.Accessibility.IndexOf(accessibility) + 1);    
                
                userResponseId = userResponseModel?.Id;
                userResponseDate = userResponseModel?.AnsweredDate;
            }
            else
            {
                var strategicResponseModel = genericRepository.GetFirstOrDefault<StrategicTraitResponse>(x =>
                    x.CandidateId == candidate.Id && x.QuestionnaireId == questionnaire.Id && x.Phase == trainingInspectionConfiguration.Accessibility.IndexOf(accessibility) + 1);
                
                userResponseId = strategicResponseModel?.Id;
                userResponseDate = strategicResponseModel?.CreatedAt;
            }
            
            result.QuestionnaireResponses.Add(new QuestionnaireResponseDto()
            {
                QuestionnaireId = questionnaire.Id,
                UserResponseId = userResponseId,
                AnsweredDate = userResponseDate?.ToFormattedDateTime(),
                EligibilityPeriod =
                    $"{accessibility.AccessPeriod.ToFormattedDateTime(false)} - {accessibility.ExpirePeriod.ToFormattedDateTime(false)}",
                IsEligible = ExtensionMethod.GetDateTimeInLocalTimeZone() >= accessibility.AccessPeriod &&
                             ExtensionMethod.GetDateTimeInLocalTimeZone() <= accessibility.ExpirePeriod
            });
        }
        
        return result;
    }

    public GetSubordinateTrainingInspectionDto GetSubordinateTrainingInspectionDetails(Guid subordinateId)
    {
        var subordinate = genericRepository.GetById<Subordinate>(subordinateId)
                          ?? throw new NotFoundException("The following subordinate could not be found.");

        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(subordinate.TrainingCandidateId)
                                ?? throw new NotFoundException("The following training candidate could not be found.");
        
        var training = genericRepository.GetById<Training>(trainingCandidate.TrainingId)
            ?? throw new NotFoundException("The following training could not be found.");
        
        var inspection = genericRepository.GetFirstOrDefault<Inspection>(x => x.InspectionType == InspectionType.PersonalAssessment)
                         ?? throw new NotFoundException("The following inspection could not be found.");
        
        var trainingInspection = genericRepository.GetFirstOrDefault<TrainingInspection>(x =>
            x.TrainingId == training.Id && x.InspectionId == inspection.Id)
                                 ?? throw new NotFoundException("The respective inspection has not been assigned to the following training.");

        var result = new GetSubordinateTrainingInspectionDto()
        {
            Id = inspection.Id,
            Name = inspection.Name,
            Description = inspection.Description,
            ImageUrl = inspection.ImageUrl,
            Type = inspection.InspectionType.ToInspectionType(),
            IsActive = inspection.IsActive,
            HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis ||
                                       genericRepository.GetCount<InspectionQuestionnaires>(z =>
                                           z.InspectionId == inspection.Id) != 0,
        };

        var trainingInspectionConfiguration = 
            trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());

        if (trainingInspectionConfiguration == null) return result;

        result.PhasesCount = trainingInspectionConfiguration.Accessibility.Count;
        
        var questionnaire = genericRepository.GetFirstOrDefault<Questionnaire>(x =>
                                x.TrainingInspectionId == trainingInspection.Id && x.IsQuestionnaireForTraining)
                            ?? throw new NotFoundException("The following questionnaire could not be found.");
        
        foreach (var accessibility in trainingInspectionConfiguration.Accessibility)
        {
            Guid? userResponseId = null;
            DateTime? userResponseDate = null;
            
            if (inspection.InspectionType != InspectionType.SwotAnalysis)
            {
                var userResponseModel = genericRepository.GetFirstOrDefault<UserResponse>(x =>
                    x.QuestionId == questionnaire.Id && x.SubordinateId == subordinate.Id && x.IsAnsweredBySubordinate && x.Phase == trainingInspectionConfiguration.Accessibility.IndexOf(accessibility) + 1);    
                
                userResponseId = userResponseModel?.Id;
                userResponseDate = userResponseModel?.AnsweredDate;
            }
            
            result.QuestionnaireResponses.Add(new SubordinateQuestionnaireResponseDto()
            {
                QuestionnaireId = questionnaire.Id,
                UserResponseId = userResponseId,
                AnsweredDate = userResponseDate?.ToFormattedDateTime(),
                SubordinateId = subordinate.Id,
                EligibilityPeriod =
                    $"{accessibility.AccessPeriod.ToFormattedDateTime(false)} - {accessibility.ExpirePeriod.ToFormattedDateTime(false)}",
                IsEligible = ExtensionMethod.GetDateTimeInLocalTimeZone() >= accessibility.AccessPeriod &&
                             ExtensionMethod.GetDateTimeInLocalTimeZone() <= accessibility.ExpirePeriod
            });
        }
        
        return result;
    }
    
    public GetCandidateTrainingInspectionDto GetCandidateTrainingInspectionDetailsForTrainingCandidate(Guid trainingCandidateId, Guid trainingInspectionId)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
                                ?? throw new NotFoundException("The following candidate has not been assigned to the respective training.");

        var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId)
            ?? throw new NotFoundException("The following candidate has not been registered to our system.");
        
        var trainingInspection = genericRepository.GetById<TrainingInspection>(trainingInspectionId)
                                 ?? throw new NotFoundException("The respective inspection has not been assigned to the following training.");

        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The following inspection could not be found.");

        var result = new GetCandidateTrainingInspectionDto()
        {
            Id = inspection.Id,
            Name = inspection.Name,
            Description = inspection.Description,
            ImageUrl = inspection.ImageUrl,
            Type = inspection.InspectionType.ToInspectionType(),
            IsActive = inspection.IsActive,
            HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis ||
                                       genericRepository.GetCount<InspectionQuestionnaires>(z =>
                                           z.InspectionId == inspection.Id) != 0,
        };
        
        var trainingInspectionConfiguration = 
            trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());

        if (trainingInspectionConfiguration == null) return result;

        result.PhasesCount = trainingInspectionConfiguration.Accessibility.Count;
        
        var questionnaire = genericRepository.GetFirstOrDefault<Questionnaire>(x =>
                                x.TrainingInspectionId == trainingInspection.Id && x.IsQuestionnaireForTraining)
                            ?? throw new NotFoundException("The following questionnaire could not be found.");
            
        foreach (var accessibility in trainingInspectionConfiguration.Accessibility)
        {
            Guid? userResponseId;
            DateTime? userResponseDate;
            
            if (inspection.InspectionType != InspectionType.SwotAnalysis)
            {
                var userResponseModel = genericRepository.GetFirstOrDefault<UserResponse>(x =>
                    x.QuestionId == questionnaire.Id && x.CandidateId == candidate.Id && x.IsAnsweredByCandidate && x.Phase == trainingInspectionConfiguration.Accessibility.IndexOf(accessibility) + 1);    
                
                userResponseId = userResponseModel?.Id;
                userResponseDate = userResponseModel?.AnsweredDate;
            }
            else
            {
                var strategicResponseModel = genericRepository.GetFirstOrDefault<StrategicTraitResponse>(x =>
                    x.CandidateId == candidate.Id && x.QuestionnaireId == questionnaire.Id && x.Phase == trainingInspectionConfiguration.Accessibility.IndexOf(accessibility) + 1);
                
                userResponseId = strategicResponseModel?.Id;
                userResponseDate = strategicResponseModel?.CreatedAt;
            }
            
            result.QuestionnaireResponses.Add(new QuestionnaireResponseDto()
            {
                QuestionnaireId = questionnaire.Id,
                UserResponseId = userResponseId,
                AnsweredDate = userResponseDate?.ToFormattedDateTime(),
                EligibilityPeriod =
                    $"{accessibility.AccessPeriod.ToFormattedDateTime(false)} - {accessibility.ExpirePeriod.ToFormattedDateTime(false)}",
                IsEligible = ExtensionMethod.GetDateTimeInLocalTimeZone() >= accessibility.AccessPeriod &&
                             ExtensionMethod.GetDateTimeInLocalTimeZone() <= accessibility.ExpirePeriod
            });
        }
        
        return result;
    }
    
    public GetTrainingInspectionQuestionnaireCountDto GetTrainingInspectionQuestionnairesCount(Guid trainingId)
    {
        var training = genericRepository.GetById<Training>(trainingId)
            ?? throw new NotFoundException("The respective training could not be found.");
        
        var trainingInspections = genericRepository.Get<TrainingInspection>(x => x.TrainingId == training.Id).ToList();

        var result = new GetTrainingInspectionQuestionnaireCountDto();
        
        foreach (var trainingInspection in trainingInspections)
        {
            var questionnaires = 
                genericRepository.Get<Questionnaire>(x => 
                    x.TrainingInspectionId == trainingInspection.Id && x.IsQuestionnaireForTraining).ToList();

            var questionDetails =
                genericRepository.Get<QuestionnaireDetails>(x =>
                    questionnaires.Select(z => z.Id).Contains(x.QuestionnaireId)).ToList();
            
            var answeredCandidates = 
                genericRepository.Get<UserResponse>(x => 
                    questionnaires.Select(z => z.Id).Contains(x.QuestionId) && x.IsAnsweredByCandidate).ToList();

            var answers = 
                genericRepository.Get<Answer>(x => 
                    x.QuestionId != null && questionDetails.Select(z => z.Id).Contains(x.QuestionId.Value)).ToList();

            result.QuestionCount += questionDetails.Count;
            result.PossibleAnswerCount += answers.Count;
            result.ResponseCount = answeredCandidates.Count;
            result.PendingAnalysisCount = 0;
        }

        return result;
    }

    public int GetTrainingInspectionPhaseCounts(Guid trainingInspectionId)
    {
        var trainingInspection = genericRepository.GetById<TrainingInspection>(trainingInspectionId)
            ?? throw new NotFoundException("The respective inspection has not been assigned to the following training.");
        
        var inspectionConfiguration =
            trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfigurationModel>(
                trainingInspection.Id, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString());
        
        return inspectionConfiguration?.Accessibility.Count ?? 0;
    }
    
    public void AssignTrainingInspections(AssignTrainingInspectionDto trainingInspections)
    {
        var training = genericRepository.GetById<Training>(trainingInspections.TrainingId)
            ?? throw new NotFoundException("The respective training could not be found.");

        var trainingInspectionsModel = genericRepository.Get<TrainingInspection>(x => 
            x.TrainingId == training.Id).ToList();
        
        var unassignedInspections = trainingInspectionsModel.Where(x => 
            !trainingInspections.InspectionId.Contains(x.InspectionId)).ToList();

        if (unassignedInspections.Select(unassignedInspection => genericRepository.GetFirstOrDefault<Questionnaire>(x => x.IsQuestionnaireForTraining && x.TrainingInspectionId == unassignedInspection.Id)).OfType<Questionnaire>().Any())
        {
            throw new BadRequestException("The following inspection could not be unassigned.", ["The following inspection already has a questionnaire linked to it."]);
        }
        
        var assignedInspections = trainingInspectionsModel.Where(x => 
            trainingInspections.InspectionId.Contains(x.InspectionId)).ToList();

        var newlyAssignedInspections = trainingInspections.InspectionId.Where(x => 
            !assignedInspections.Select(z => z.InspectionId).Contains(x)).ToList();

        var unassignedInspectionsConfigurations = genericRepository
            .Get<TrainingInspectionConfiguration>(x =>
                unassignedInspections.Select(z => z.Id).Contains(x.TrainingInspectionId)).ToList();
        
        if (unassignedInspectionsConfigurations.Count != 0)
        {
            genericRepository.RemoveMultipleEntity(unassignedInspectionsConfigurations);
        }
        
        if (unassignedInspections.Count != 0)
        {
            genericRepository.RemoveMultipleEntity(unassignedInspections);
        }

        foreach (var inspection in newlyAssignedInspections.Select(inspectionId => genericRepository.GetById<Inspection>(inspectionId)))
        {
            if (inspection == null)
                throw new NotFoundException("The respective inspection could not be found.");

            var trainingInspection = new TrainingInspection()
            {
                TrainingId = training.Id,
                InspectionId = inspection.Id
            };

            var trainingInspectionId = genericRepository.Insert(trainingInspection);
            
            if (inspection.InspectionType == InspectionType.SwotAnalysis)
            {
                var questionnaire = new Questionnaire()
                {
                    TrainingInspectionId = trainingInspectionId,
                    IsQuestionnaireForTraining = true,
                };

                genericRepository.Insert(questionnaire);
            }

            var trainingInspectionConfiguration = new TrainingInspectionConfigurationModel()
            {
                Accessibility =
                [
                    new AbstractTrainingInspectionConfigurationDto
                    {
                        AccessPeriod = DateTime.UtcNow,
                        ExpirePeriod = DateTime.UtcNow.AddDays(7)
                    }
                ]
            };
            
            trainingInspectionConfigurationService.SaveProperty(trainingInspectionId, TrainingInspectionConfigurationModule.RESPONSE_PERIOD.ToString(), trainingInspectionConfiguration);
        }
    }

    public async Task TriggerTrainingInspectionQuestionnaireForSubordinates(Guid trainingInspectionConfigurationId, string recurringJobId)
    {
        var trainingInspectionConfiguration = genericRepository.GetById<TrainingInspectionConfiguration>(trainingInspectionConfigurationId)
            ?? throw new NotFoundException("The respective inspection configuration could not be found.");
        
        var trainingInspection = genericRepository.GetById<TrainingInspection>(trainingInspectionConfiguration.TrainingInspectionId)
            ?? throw new NotFoundException("The respective inspection has not been assigned to the following training.");
        
        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
            ?? throw new NotFoundException("The respective inspection could not be found.");
        
        var questionnaire = genericRepository.GetFirstOrDefault<Questionnaire>(x => x.TrainingInspectionId == trainingInspection.Id)
            ?? throw new NotFoundException("The following questionnaire has not been assigned to the respective training inspection.");
        
        var training = genericRepository.GetById<Training>(trainingInspection.TrainingId)
            ?? throw new NotFoundException("The respective training could not be found.");

        var trainingCandidates = genericRepository
            .Get<TrainingCandidate>(x => x.TrainingId == training.Id && x.IsApproved).ToList();

        foreach (var trainingCandidate in trainingCandidates)
        {
            var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId)
                ?? throw new NotFoundException("The following candidate could not be found.");

            var subordinates = genericRepository.Get<Subordinate>(x => 
                    x.TrainingCandidateId == trainingCandidate.Id).ToList();

            foreach (var subordinate in subordinates)
            {
                var fullUrl = $"{_baseUrl}/{Constants.Navigation.SubordinateAnswerUploadForm}/{questionnaire.Id}/{subordinate.Id}";

                var emailDto = new EmailDto
                {
                    FullName = subordinate.Name,
                    UserName = candidate.Name,
                    ToEmailAddress = subordinate.Email,
                    Subject = $"Connect CMS - {inspection.Name}",
                    PrimaryMessage = fullUrl,
                    EmailProcess = EmailProcess.SubordinatesQuestionnaire,
                };

                await emailService.SendEmail(emailDto);
            }
        }
        
        hangfireService.RemoveRecurringJobs(recurringJobId);
    }
}