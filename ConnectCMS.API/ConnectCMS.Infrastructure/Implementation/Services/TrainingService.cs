using CMSTrain.Helper;
using CMSTrain.Domain.Common;
using System.Linq.Expressions;
using CMSTrain.Domain.Entities;
using CMSTrain.Application.DTOs.Count;
using CMSTrain.Application.Exceptions;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Training;
using CMSTrain.Application.DTOs.ClassTrainers;
using CMSTrain.Application.DTOs.Configuration.Class;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.Configuration.Training;
using CMSTrain.Application.DTOs.Organization;
using CMSTrain.Application.Interfaces.Repositories.Base;
using TrainingConfiguration = CMSTrain.Domain.Common.Enum.Configurations.TrainingConfiguration;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class TrainingService(IGenericRepository genericRepository, 
    ITrainingConfigurationService trainingConfigurationService,
    IClassConfigurationService classConfigurationService,
    IFileService fileService) : ITrainingService
{
    private const string TrainingFilePath = Constants.FilePath.TrainingsImagesFilePath;

    public List<GetTrainingDto> GetAllTrainings(int statusAction, int pageNumber, int pageSize, out int rowCount, string? search, bool? isActive)
    {
        var condition = GetTrainingDetailsCondition(statusAction, search, isActive);
        
        var trainings = genericRepository.GetPagedResult(pageNumber, pageSize, out rowCount, condition).ToList();

        var result = trainings.Select(x => new GetTrainingDto()
        {
            Id = x.Id,
            Description = x.Description,
            ImageUrl = x.ImageUrl,
            Title = x.Title,
            Longitude = x.Longitude ?? 0m,
            Latitude = x.Latitude ?? 0m,
            LocationDetails = x.LocationDetails,
            StartDate = x.StartDate.ToFormattedDate(),
            EndDate = x.EndDate.ToFormattedDate(),
            TrainingFormatId = x.TrainingFormatId,
            TrainingFormat = genericRepository.GetById<TrainingFormat>(x.TrainingFormatId)!.Name,
            IsActive = x.IsActive,
            AssignedTrainers = GetAssignedTrainingsTrainers(x.Id)
        }).ToList();

        return result;
    }

    public List<GetTrainingDto> GetAllTrainings(int statusAction, string? search, bool? isActive)
    {
        var condition = GetTrainingDetailsCondition(statusAction, search, isActive);

        var result = new List<GetTrainingDto>();

        var trainings = genericRepository.Get(condition).ToList();

        foreach (var training in trainings)
        {
            var trainingDto = new GetTrainingDto()
            {
                Id = training.Id,
                Description = training.Description,
                ImageUrl = training.ImageUrl,
                Title = training.Title,
                LocationDetails = training.LocationDetails,
                Longitude = training.Longitude ?? 0m,
                Latitude = training.Latitude ?? 0m,
                StartDate = training.StartDate.ToFormattedDate(),
                EndDate = training.EndDate.ToFormattedDate(),
                TrainingFormatId = training.TrainingFormatId,
                TrainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)!.Name,
                IsActive = training.IsActive,
                AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
            };

            result.Add(trainingDto);
        }

        return result;
    }

    public AssignedTrainingCountDto GetAvailableTrainingsCount()
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        return new AssignedTrainingCountDto
        {
            AllCount = genericRepository.GetCount<Training>(),
            AvailableCount = genericRepository.GetCount<Training>(x => x.EndDate >= currentDate),
            ExpiredCount = genericRepository.GetCount<Training>(x => x.EndDate < currentDate)
        };    
    }
    
    public GetTrainingDto GetTrainingById(Guid id)
    {
        var training = genericRepository.GetById<Training>(id)
            ?? throw new NotFoundException("The following training was not found.");

        return new GetTrainingDto()
        {
            Id = training.Id,
            Description = training.Description,
            ImageUrl = training.ImageUrl,
            Title = training.Title,
            Longitude = training.Longitude ?? 0m,
            Latitude = training.Latitude ?? 0m,
            StartDate = training.StartDate.ToFormattedDate(),
            EndDate = training.EndDate.ToFormattedDate(),
            TrainingFormatId = training.TrainingFormatId,
            TrainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)?.Name ?? "",
            LocationDetails = training.LocationDetails,
            IsActive = training.IsActive,
            AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
        };
    }

    public GetTrainingDto GetTrainingDetailsByInspection(Guid trainingInspectionId)
    {
        var trainingInspection = genericRepository.GetById<TrainingInspection>(trainingInspectionId)
                                 ?? throw new NotFoundException(
                                     "The following training inspection has not been allocated to any of the following training.");
        
        var training = genericRepository.GetById<Training>(trainingInspection.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        return new GetTrainingDto()
        {
            Id = training.Id,
            Title = training.Title,
            Description = training.Description,
            ImageUrl = training.ImageUrl,
            Longitude = training.Longitude ?? 0m,
            Latitude = training.Latitude ?? 0m,
            StartDate = training.StartDate.ToFormattedDate(),
            EndDate = training.EndDate.ToFormattedDate(),
        };
    }

    public GetTrainingDto GetTrainingDetailsByQuestionnaire(Guid questionnaireId)
    {
        var questionnaire = genericRepository.GetById<Questionnaire>(questionnaireId)
                            ?? throw new NotFoundException("The following questionnaire could not be found.");

        if (questionnaire.TrainingInspectionId == null)
            throw new NotFoundException("The following questionnaire has not been assigned to any training(s).");

        var trainingInspection = genericRepository.GetById<TrainingInspection>(questionnaire.TrainingInspectionId)
                                 ?? throw new NotFoundException(
                                     "The following training inspection has not been allocated to any of the following training.");
        
        var training = genericRepository.GetById<Training>(trainingInspection.TrainingId)
            ?? throw new NotFoundException("The following training could not be found.");

        return new GetTrainingDto()
        {
            Id = training.Id,
            Title = training.Title,
            Description = training.Description,
            ImageUrl = training.ImageUrl,
            Longitude = training.Longitude ?? 0m,
            Latitude = training.Latitude ?? 0m,
            StartDate = training.StartDate.ToFormattedDate(),
            EndDate = training.EndDate.ToFormattedDate(),
        };
    }
    
    public TrainingModuleCountDto GetTrainingModuleCount(bool? isActive = null)
    {
        return new TrainingModuleCountDto
        {
            AllCount = genericRepository.GetCount<Training>(x => isActive == null || x.IsActive == isActive),
            AvailableCount = genericRepository.GetCount<Training>(x => x.EndDate >= DateOnly.FromDateTime(DateTime.Now) && (isActive == null || x.IsActive == isActive)),
            ExpiredCount = genericRepository.GetCount<Training>(x => x.StartDate < DateOnly.FromDateTime(DateTime.Now) && x.EndDate < DateOnly.FromDateTime(DateTime.Now) && (isActive == null || x.IsActive == isActive)),
        };
    }

    public TrainingDetailsCountDto GetTrainingDetailsCount(Guid trainingId)
    {
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training was not found.");

        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();

        var classesCount = classes.Count;

        var resourceMaterialsCount = genericRepository.GetCount<TrainingResources>(x => 
            x.TrainingId == training.Id);

        var candidatesCount = genericRepository.GetCount<TrainingCandidate>(x => 
            x.TrainingId == training.Id && x.IsActionCompleted && x.IsApproved);

        var trainersCount = genericRepository.Get<ClassTrainer>(x => 
            classes.Select(z => z.Id).Contains(x.ClassId)).ToList().DistinctBy(x => x.TrainerId).Count();

        var trainingInspectionCount = genericRepository.GetCount<TrainingInspection>(x => 
            x.TrainingId == training.Id);
        
        return new TrainingDetailsCountDto
        {
            ClassDetailsCount = classesCount,
            ResourceDetailsCount = resourceMaterialsCount,
            CandidateDetailsCount = candidatesCount,
            TrainerDetailsCount = trainersCount,
            InspectionCount = trainingInspectionCount
        };
    }
    
    public List<GetOrganizationDto> GetAllAssignedClientOrganizations(Guid trainingId)
    {
        var training = genericRepository.GetById<Training>(trainingId)
            ?? throw new NotFoundException("The following training was not found.");

        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x => x.TrainingId == training.Id && x.IsApproved).ToList();
        
        var candidates = genericRepository.Get<User>(x => 
            trainingCandidates.Select(z => z.CandidateId).Contains(x.Id)).ToList();
        
        var organizations = genericRepository.Get<Organization>(x => 
            candidates.Select(z => z.OrganizationId).Contains(x.Id)).ToList();
        
        return organizations.Select(x => new GetOrganizationDto()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            ImageUrl = x.ImageUrl,
            IsActive = x.IsActive
        }).ToList();
    }
    
    public void InsertTraining(CreateTrainingDto training)
    {
        var trainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)
            ?? throw new NotFoundException("The following training format was not found.");

        if (training.StartDate.Date < DateTime.UtcNow.Date)
        {
            throw new BadRequestException("The following training could not be created.",
                ["The start date is not valid, it should be greater or equal to today's date."]);
        }

        if (training.EndDate.Date < training.StartDate.Date)
        {
            throw new BadRequestException("The following training could not be created.",
                ["The end date is not valid, it should be greater or equal to the start date of the training."]);
        }

        var trainingImage = "";

        if (training.Image != null)
        {
            trainingImage = fileService.UploadDocument(training.Image, TrainingFilePath);
        }

        var trainingModel = new Training()
        {
            Title = training.Title,
            Description = training.Description,
            ImageUrl = !string.IsNullOrEmpty(trainingImage) ? trainingImage : null,
            StartDate = DateOnly.FromDateTime(training.StartDate),
            EndDate = DateOnly.FromDateTime(training.EndDate),
            Longitude = training.Longitude,
            Latitude = training.Latitude,
            TrainingFormatId = trainingFormat.Id,
            LocationDetails = training.LocationDetails
        };

        var trainingId = genericRepository.Insert(trainingModel);
        
        var trainingResourceConfiguration = new TrainingResourceConfiguration()
        {
            Accessibility = new AbstractTrainingResourceConfigurationDto()
            {
                AccessPeriod = trainingModel.StartDate.AddDays(-1).ToDateTime(TimeOnly.MinValue),
                ExpirePeriod = trainingModel.EndDate.AddMonths(6).ToDateTime(TimeOnly.MinValue)
            }
        };
        
        trainingConfigurationService.SaveProperty(trainingId, TrainingConfiguration.RESOURCE_AVAILABILITY.ToString(), trainingResourceConfiguration);

        var classModel = new Class()
        {
            Title = $"{trainingModel.Title} - Class #1",
            IsDefaultClass = true,
            Date = trainingModel.StartDate,
            ImageUrl = null,
            TrainingId = trainingId,
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
        };

        var classId = genericRepository.Insert(classModel);
        
        var classResourceConfiguration = new ClassResourceConfiguration()
        {
            Accessibility = new AbstractClassResourceConfigurationDto()
            {
                AccessPeriod = trainingModel.StartDate.AddDays(-1).ToDateTime(TimeOnly.MinValue),
                ExpirePeriod = trainingModel.EndDate.AddMonths(6).ToDateTime(TimeOnly.MinValue)
            }
        };

        var classAttendanceConfiguration = new ClassAttendanceConfiguration()
        {
            Accessibility = new AbstractClassAttendanceConfigurationDto()
            {
                Date = classModel.Date.ToDateTime(TimeOnly.MinValue),
                AccessPeriod = classModel.StartTime.Add(TimeSpan.FromMinutes(-30)),
                ExpirePeriod = classModel.EndTime.Add(TimeSpan.FromMinutes(30)),
                IsLocationEnabled = false,
                Radius = null
            }
        };

        classConfigurationService.SaveProperty(classId, Domain.Common.Enum.Configurations.ClassConfiguration.RESOURCE_AVAILABILITY.ToString(),
            classResourceConfiguration);
        
        classConfigurationService.SaveProperty(classId, Domain.Common.Enum.Configurations.ClassConfiguration.ATTENDANCE_PERIOD.ToString(),
            classAttendanceConfiguration);

        var trainingCertificationConfiguration = new TrainingCertificationConfigurationUpload()
        {
            Certification = new AbstractTrainingCertificationConfigurationUploadDto()
            {
                PrimaryColor = "#18B2E6",
                SecondaryColor = "#18B2E6",
                TertiaryColor = "#AEFEFD",
            }
        };
        
        trainingConfigurationService.SavePropertyDetails(trainingId, TrainingConfiguration.CERTIFICATIONS.ToString(), trainingCertificationConfiguration);
        
        var trainingCertificationTriggerConfiguration = new TrainingCertificationTriggerConfiguration()
        {
            Trigger = new AbstractTrainingCertificationTriggerConfigurationDto()
            {
                IsManual = true
            }
        };
        
        trainingConfigurationService.SavePropertyDetails(trainingId, TrainingConfiguration.CERTIFICATION_TRIGGER.ToString(), trainingCertificationTriggerConfiguration);
    }

    public void UpdateTraining(UpdateTrainingDto training)
    {
        var trainingModel = genericRepository.GetById<Training>(training.Id)
            ?? throw new NotFoundException("The following training was not found.");

        var existingTrainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)
            ?? throw new NotFoundException("The following training format was not found.");

        if (training.StartDate.Date < DateTime.UtcNow.Date)
        {
            throw new BadRequestException("The following training could not be created.",
                ["The start date is not valid, it should be greater or equal to today's date."]);
        }

        if (training.EndDate.Date < training.StartDate.Date)
        {
            throw new BadRequestException("The following training could not be created.",
                ["The end date is not valid, it should be greater or equal to the start date of the training."]);
        }

        trainingModel.Title = training.Title;
        trainingModel.Description = training.Description;
        trainingModel.LocationDetails = training.LocationDetails;
        trainingModel.Longitude = training.Longitude;
        trainingModel.Latitude = training.Latitude;
        trainingModel.TrainingFormatId = existingTrainingFormat.Id;
        trainingModel.EndDate = DateOnly.FromDateTime(training.EndDate);
        trainingModel.StartDate = DateOnly.FromDateTime(training.StartDate);

        if (training.Image != null)
        {
            if (!string.IsNullOrEmpty(trainingModel.ImageUrl))
            {
                var trainingPathPath = Path.Combine(TrainingFilePath, trainingModel.ImageUrl);

                fileService.DeleteFile(trainingPathPath);
            }

            trainingModel.ImageUrl = fileService.UploadDocument(training.Image, TrainingFilePath);
        }

        genericRepository.Update(trainingModel);
        
        var trainingResourceConfiguration = new TrainingResourceConfiguration()
        {
            Accessibility = new AbstractTrainingResourceConfigurationDto()
            {
                AccessPeriod = trainingModel.StartDate.AddDays(-1).ToDateTime(TimeOnly.MinValue),
                ExpirePeriod = trainingModel.EndDate.AddMonths(6).ToDateTime(TimeOnly.MinValue)
            }
        };
        
        trainingConfigurationService.SaveProperty(trainingModel.Id, TrainingConfiguration.RESOURCE_AVAILABILITY.ToString(), trainingResourceConfiguration);

        var @class = genericRepository.GetFirstOrDefault<Class>(x => 
            x.TrainingId == training.Id && x.IsDefaultClass != null && x.IsDefaultClass.Value == true);

        if (@class == null) return;
        
        @class.Date = trainingModel.StartDate;
        @class.Title = $"{trainingModel.Title} - Class #1";

        genericRepository.Update(@class);
            
        var classResourceConfiguration = new ClassResourceConfiguration()
        {
            Accessibility = new AbstractClassResourceConfigurationDto()
            {
                AccessPeriod = trainingModel.StartDate.AddDays(-1).ToDateTime(TimeOnly.MinValue),
                ExpirePeriod = trainingModel.EndDate.AddMonths(6).ToDateTime(TimeOnly.MinValue)
            }
        };

        var classAttendanceConfiguration = new ClassAttendanceConfiguration()
        {
            Accessibility = new AbstractClassAttendanceConfigurationDto()
            {
                Date = @class.Date.ToDateTime(TimeOnly.MinValue),
                AccessPeriod = @class.StartTime.Add(TimeSpan.FromMinutes(-30)),
                ExpirePeriod = @class.EndTime.Add(TimeSpan.FromMinutes(30)),
                IsLocationEnabled = false,
                Radius = null
            }
        };

        classConfigurationService.SaveProperty(@class.Id, Domain.Common.Enum.Configurations.ClassConfiguration.RESOURCE_AVAILABILITY.ToString(),
            classResourceConfiguration);
        
        classConfigurationService.SaveProperty(@class.Id, Domain.Common.Enum.Configurations.ClassConfiguration.ATTENDANCE_PERIOD.ToString(),
            classAttendanceConfiguration);
    }

    public void ActivateDeactivateTraining(Guid id)
    {
        var trainingModel = genericRepository.GetById<Training>(id)
            ?? throw new NotFoundException("The following training was not found.");

        trainingModel.IsActive = !trainingModel.IsActive;

        genericRepository.Update(trainingModel);
    }

    private static Expression<Func<Training, bool>> GetTrainingDetailsCondition(int statusAction, string? search, bool? isActive)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        Expression<Func<Training, bool>> condition = 
            statusAction switch
            {
                Constants.StatusAction.Available => x => x.EndDate >= currentDate && (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())) && (isActive == null || x.IsActive == isActive),
                Constants.StatusAction.Expired => x => x.StartDate < currentDate && x.EndDate < currentDate && (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())) && (isActive == null || x.IsActive == isActive),
                _ => x => (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())) && (isActive == null || x.IsActive == isActive)
            };

        return condition;
    }
    
    private GetAssignedTrainingsTrainersDto GetAssignedTrainingsTrainers(Guid trainingId)
    {
        var training = genericRepository.GetById<Training>(trainingId) 
                       ?? throw new NotFoundException("The following training was not found.");
        
        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList(); 
        
        var classTrainers = genericRepository.Get<ClassTrainer>(x => classes.Select(z => z.Id).Contains(x.ClassId)).ToList();

        var trainers = genericRepository.Get<User>(x => classTrainers.Select(z => z.TrainerId).Contains(x.Id)).ToList();
        
        var trainersList = trainers.Select(trainersDetails => new GetTrainersDto()
        {
            Id = trainersDetails.Id,
            Name = trainersDetails.Name,
            ImageUrl = trainersDetails.ImageURL,
            Username = trainersDetails.UserName ?? string.Empty,
            EmailAddress = trainersDetails.Email ?? string.Empty,
            PhoneNumber = trainersDetails.PhoneNumber ?? string.Empty,
        }).ToList();

        var classCount = classes.Count switch
        {
            0 => "No classes assigned",
            1 => "1 Class",
            _ => $"{classes.Count - 1}+ Classes"
        };

        return new GetAssignedTrainingsTrainersDto()
        {
            Trainers = trainersList,
            ClassCount = classCount
        };
    }
}
