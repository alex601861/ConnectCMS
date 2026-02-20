using CMSTrain.Helper;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Application.DTOs.Class;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Common.User;
using CMSTrain.Application.DTOs.Count;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.Configuration.Class;
using CMSTrain.Application.Interfaces.Repositories.Base;
using ClassConfiguration = CMSTrain.Domain.Common.Enum.Configurations.ClassConfiguration;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class ClassService(
    IGenericRepository genericRepository,
    IClassConfigurationService classConfigurationService,
    ICurrentUserService userService,
    IFileService fileService) : IClassService
{
    private const string ClassesFilePath = Constants.FilePath.ClassesImagesFilePath;

    public List<GetClassDto> GetAllClasses(Guid trainingId)
    {
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();

        return classes.Select(@class => new GetClassDto
        {
            Id = @class.Id,
            Title = @class.Title,
            TrainingId = @class.TrainingId,
            Date = @class.Date.ToFormattedDate(),
            StartTime = @class.StartTime.ToFormattedTime(),
            EndTime = @class.EndTime.ToFormattedTime(),
            Training = genericRepository.GetById<Training>(@class.TrainingId)!.Title,
            AssignedTrainers = genericRepository.Get<ClassTrainer>(x => x.ClassId == @class.Id).Count(),
            Status = GetClassStatus(@class.Date, @class.StartTime, @class.EndTime),
            ImageUrl = @class.ImageUrl
        }).ToList();
    }

    public List<GetClassDto> GetAllClasses(Guid trainingId, int pageNumber, int pageSize, out int rowCount,
        string? search = null, int? status = null)
    {
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        
        var currentTimeSpan = DateTime.Now.TimeOfDay;

        var classes = status switch
        {
            Constants.Schedule.Scheduled => genericRepository.GetPagedResult<Class>(pageNumber, pageSize, out rowCount,
                    x => x.TrainingId == training.Id
                         && (x.Date > currentDate || (x.Date == currentDate && x.StartTime > currentTimeSpan))
                         && (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())))
                .ToList(),
            Constants.Schedule.InProgress => genericRepository.GetPagedResult<Class>(pageNumber, pageSize, out rowCount,
                    x => x.TrainingId == training.Id
                         && currentTimeSpan >= x.StartTime
                         && currentTimeSpan <= x.EndTime
                         && x.Date == currentDate
                         && (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())))
                .ToList(),
            Constants.Schedule.Completed => genericRepository.GetPagedResult<Class>(pageNumber, pageSize, out rowCount,
                    x => x.TrainingId == training.Id
                         && (x.Date < currentDate || (x.Date == currentDate && currentTimeSpan >= x.EndTime))
                         && (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())))
                .ToList(),
            _ => genericRepository.GetPagedResult<Class>(pageNumber, pageSize, out rowCount,
                    x => x.TrainingId == training.Id &&
                         (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())))
                .ToList()
        };

        return classes.Select(@class => new GetClassDto
        {
            Id = @class.Id,
            Title = @class.Title,
            TrainingId = @class.TrainingId,
            Date = @class.Date.ToFormattedDate(),
            StartTime = @class.StartTime.ToFormattedTime(),
            EndTime = @class.EndTime.ToFormattedTime(),
            Training = genericRepository.GetById<Training>(@class.TrainingId)!.Title,
            AssignedTrainers = genericRepository.Get<ClassTrainer>(x => x.ClassId == @class.Id).Count(),
            Status = GetClassStatus(@class.Date, @class.StartTime, @class.EndTime),
            ImageUrl = @class.ImageUrl
        }).ToList();
    }

    public List<GetClassForTrainersDto> GetAllClassesForTrainers(Guid trainingId)
    {
        var trainerId = userService.GetUserId;

        var trainer = genericRepository.GetById<User>(trainerId)
                      ?? throw new NotFoundException("The following trainer has not been registered to our system.");

        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found");

        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();

        var trainerClasses = genericRepository
            .Get<ClassTrainer>(x => x.TrainerId == trainer.Id && classes.Select(z => z.Id).Contains(x.ClassId))
            .ToList();

        var result = new List<GetClassForTrainersDto>();

        foreach (var classModule in trainerClasses)
        {
            var @class = genericRepository.GetById<Class>(classModule.Id)
                         ?? throw new NotFoundException("The following class could not be found.");

            var trainingCandidates =
                genericRepository.Get<TrainingCandidate>(x =>
                    x.TrainingId == training.Id && x.IsApproved && x.IsActionCompleted).ToList();

            result.Add(new GetClassForTrainersDto
            {
                Id = @class.Id,
                Title = @class.Title,
                TrainingId = @class.TrainingId,
                Date = @class.Date.ToFormattedDate(),
                StartTime = @class.StartTime.ToFormattedTime(),
                EndTime = @class.EndTime.ToFormattedTime(),
                ImageUrl = @class.ImageUrl,
                Training = genericRepository.GetById<Training>(@class.TrainingId)!.Title,
                AssignedTrainers = genericRepository.Get<ClassTrainer>(x => x.ClassId == @class.Id).Count(),
                Status = GetClassStatus(@class.Date, @class.StartTime, @class.EndTime),
                TotalApprovedCandidates = trainingCandidates.Count,
                TotalAttendedCandidates = GetAttendanceCount(@class.Id),
                TotalAcceptedAttendanceCount = GetAttendanceCount(@class.Id, Constants.RequestAction.Accepted),
                TotalPendingAttendanceCount = GetAttendanceCount(@class.Id, Constants.RequestAction.Pending),
                TotalRejectedAttendanceCount = GetAttendanceCount(@class.Id, Constants.RequestAction.Rejected),
            });
        }

        return result;
    }

    public List<GetClassForTrainersDto> GetAllClassesForTrainers(Guid trainingId, int pageNumber, int pageSize,
        out int rowCount, string? search = null, int? status = null)
    {
        var trainerId = userService.GetUserId;

        var trainer = genericRepository.GetById<User>(trainerId)
                      ?? throw new NotFoundException("The following trainer has not been registered to our system.");

        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found");

        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        TimeSpan currentTimeSpan = DateTime.Now.TimeOfDay;

        var classes = status switch
        {
            Constants.Schedule.Scheduled => genericRepository.GetPagedResult<Class>(pageNumber, pageSize, out rowCount,
                    x => x.TrainingId == training.Id
                         && (x.Date > currentDate || (x.Date == currentDate && x.StartTime > currentTimeSpan))
                         && (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())))
                .ToList(),
            Constants.Schedule.InProgress => genericRepository.GetPagedResult<Class>(pageNumber, pageSize, out rowCount,
                    x => x.TrainingId == training.Id
                         && currentTimeSpan >= x.StartTime
                         && currentTimeSpan <= x.EndTime
                         && x.Date == currentDate
                         && (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())))
                .ToList(),
            Constants.Schedule.Completed => genericRepository.GetPagedResult<Class>(pageNumber, pageSize, out rowCount,
                    x => x.TrainingId == training.Id
                         && (x.Date < currentDate || (x.Date == currentDate && currentTimeSpan >= x.EndTime))
                         && (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())))
                .ToList(),
            _ => genericRepository.GetPagedResult<Class>(pageNumber, pageSize, out rowCount,
                    x => x.TrainingId == training.Id &&
                         (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())))
                .ToList()
        };

        var trainerClasses = genericRepository
            .Get<ClassTrainer>(x => x.TrainerId == trainer.Id && classes.Select(z => z.Id).Contains(x.ClassId))
            .ToList();

        var result = new List<GetClassForTrainersDto>();

        foreach (var classModule in trainerClasses)
        {
            var @class = genericRepository.GetById<Class>(classModule.ClassId)
                         ?? throw new NotFoundException("The following class could not be found.");

            var trainingCandidates =
                genericRepository.Get<TrainingCandidate>(x =>
                    x.TrainingId == training.Id && x.IsApproved && x.IsActionCompleted).ToList();

            result.Add(new GetClassForTrainersDto
            {
                Id = @class.Id,
                Title = @class.Title,
                TrainingId = @class.TrainingId,
                Date = @class.Date.ToFormattedDate(),
                StartTime = @class.StartTime.ToFormattedTime(),
                EndTime = @class.EndTime.ToFormattedTime(),
                ImageUrl = @class.ImageUrl,
                Training = genericRepository.GetById<Training>(@class.TrainingId)!.Title,
                AssignedTrainers = genericRepository.Get<ClassTrainer>(x => x.ClassId == @class.Id).Count(),
                Status = GetClassStatus(@class.Date, @class.StartTime, @class.EndTime),
                TotalApprovedCandidates = trainingCandidates.Count,
                TotalAttendedCandidates = GetAttendanceCount(@class.Id),
                TotalAcceptedAttendanceCount = GetAttendanceCount(@class.Id, Constants.RequestAction.Accepted),
                TotalPendingAttendanceCount = GetAttendanceCount(@class.Id, Constants.RequestAction.Pending),
                TotalRejectedAttendanceCount = GetAttendanceCount(@class.Id, Constants.RequestAction.Rejected),
            });
        }

        return result;
    }

    public List<GetClassForCandidatesDto> GetAllClassesForCandidates(Guid trainingId)
    {
        var training = genericRepository.GetById<Training>(trainingId) ??
                       throw new NotFoundException("The following training could not be found.");

        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();

        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId) ??
                        throw new NotFoundException("The following candidate has not been registered to our system.");

        return classes.Select(@class => new GetClassForCandidatesDto
        {
            Id = @class.Id,
            Title = @class.Title,
            TrainingId = @class.TrainingId,
            Date = @class.Date.ToFormattedDate(),
            StartTime = @class.StartTime.ToFormattedTime(),
            EndTime = @class.EndTime.ToFormattedTime(),
            ImageUrl = @class.ImageUrl,
            Training = genericRepository.GetById<Training>(@class.TrainingId)!.Title,
            AssignedTrainers = genericRepository.Get<ClassTrainer>(x => x.ClassId == @class.Id).Count(),
            Status = GetClassStatus(@class.Date, @class.StartTime, @class.EndTime),
            AttendanceId = GetAttendanceStatus(@class.Id, candidate.Id),
            AttendanceApprovedStatus = GetAttendanceApprovalStatus(@class.Id, candidate.Id),
            AttendanceMarkedStatus = GetAttendanceMarkedStatus(@class.Id, candidate.Id)
        }).ToList();
    }

    public List<GetClassForCandidatesDto> GetAllClassesForCandidates(Guid trainingId, int pageNumber, int pageSize,
        out int rowCount, string? search = null, int? status = null)
    {
        var training = genericRepository.GetById<Training>(trainingId) ??
                       throw new NotFoundException("The following training could not be found.");

        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        TimeSpan currentTimeSpan = DateTime.Now.TimeOfDay;

        var classes = status switch
        {
            Constants.Schedule.Scheduled => genericRepository.GetPagedResult<Class>(pageNumber, pageSize, out rowCount,
                    x => x.TrainingId == training.Id
                         && (x.Date > currentDate || (x.Date == currentDate && x.StartTime > currentTimeSpan))
                         && (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())))
                .ToList(),
            Constants.Schedule.InProgress => genericRepository.GetPagedResult<Class>(pageNumber, pageSize, out rowCount,
                    x => x.TrainingId == training.Id
                         && currentTimeSpan >= x.StartTime
                         && currentTimeSpan <= x.EndTime
                         && x.Date == currentDate
                         && (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())))
                .ToList(),
            Constants.Schedule.Completed => genericRepository.GetPagedResult<Class>(pageNumber, pageSize, out rowCount,
                    x => x.TrainingId == training.Id
                         && (x.Date < currentDate || (x.Date == currentDate && currentTimeSpan >= x.EndTime))
                         && (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())))
                .ToList(),
            _ => genericRepository.GetPagedResult<Class>(pageNumber, pageSize, out rowCount,
                    x => x.TrainingId == training.Id &&
                         (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())))
                .ToList()
        };

        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId) ??
                        throw new NotFoundException("The following candidate has not been registered to our system.");

        return classes.Select(x => new GetClassForCandidatesDto()
        {
            Id = x.Id,
            Title = x.Title,
            TrainingId = x.TrainingId,
            Date = x.Date.ToFormattedDate(),
            StartTime = x.StartTime.ToFormattedTime(),
            EndTime = x.EndTime.ToFormattedTime(),
            ImageUrl = x.ImageUrl,
            Training = genericRepository.GetById<Training>(x.TrainingId)!.Title,
            AssignedTrainers = genericRepository.Get<ClassTrainer>(z => z.ClassId == x.Id).Count(),
            Status = GetClassStatus(x.Date, x.StartTime, x.EndTime),
            AttendanceId = GetAttendanceStatus(x.Id, candidate.Id),
            AttendanceApprovedStatus = GetAttendanceApprovalStatus(x.Id, candidate.Id),
            AttendanceMarkedStatus = GetAttendanceMarkedStatus(x.Id, candidate.Id)
        }).ToList();
    }

    public List<GetClassForCandidatesDto> GetAllCandidateClasses(Guid trainingCandidateId)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been registered to the respective training.");

        var training = genericRepository.GetById<Training>(trainingCandidate.TrainingId) ??
                       throw new NotFoundException("The following training could not be found.");

        var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId) ??
                        throw new NotFoundException("The following candidate has not been registered to our system.");

        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();

        var result = new List<GetClassForCandidatesDto>();

        foreach (var @class in classes)
        {
            var classTrainers = genericRepository.Get<ClassTrainer>(x => x.ClassId == @class.Id).ToList();

            result.Add(new GetClassForCandidatesDto()
            {
                Id = @class.Id,
                Title = @class.Title,
                TrainingId = training.Id,
                Training = training.Title,
                AssignedTrainers = classTrainers.Count,
                StartTime = @class.StartTime.ToFormattedTime(),
                EndTime = @class.EndTime.ToFormattedTime(),
                ImageUrl = @class.ImageUrl,
                Status = GetClassStatus(@class.Date, @class.StartTime, @class.EndTime),
                Date = @class.Date.ToFormattedDate(),
                AttendanceId = GetAttendanceStatus(@class.Id, candidate.Id),
                AttendanceApprovedStatus = GetAttendanceApprovalStatus(@class.Id, candidate.Id),
                AttendanceMarkedStatus = GetAttendanceMarkedStatus(@class.Id, candidate.Id)
            });
        }

        return result;
    }

    public List<GetClassForCandidatesDto> GetAllCandidateClasses(Guid trainingCandidateId, int pageNumber, int pageSize,
        out int rowCount)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been registered to the respective training.");

        var training = genericRepository.GetById<Training>(trainingCandidate.TrainingId) ??
                       throw new NotFoundException("The following training could not be found.");

        var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId) ??
                        throw new NotFoundException("The following candidate has not been registered to our system.");

        var classes = genericRepository
            .GetPagedResult<Class>(pageNumber, pageSize, out rowCount, x => x.TrainingId == training.Id).ToList();

        var result = new List<GetClassForCandidatesDto>();

        foreach (var @class in classes)
        {
            var classTrainers = genericRepository.Get<ClassTrainer>(x => x.ClassId == @class.Id).ToList();

            result.Add(new GetClassForCandidatesDto()
            {
                Id = @class.Id,
                Title = @class.Title,
                TrainingId = training.Id,
                Training = training.Title,
                AssignedTrainers = classTrainers.Count,
                StartTime = @class.StartTime.ToFormattedTime(),
                EndTime = @class.EndTime.ToFormattedTime(),
                ImageUrl = @class.ImageUrl,
                Status = GetClassStatus(@class.Date, @class.StartTime, @class.EndTime),
                Date = @class.Date.ToFormattedDate(),
                AttendanceId = GetAttendanceStatus(@class.Id, candidate.Id),
                AttendanceApprovedStatus = GetAttendanceApprovalStatus(@class.Id, candidate.Id),
                AttendanceMarkedStatus = GetAttendanceMarkedStatus(@class.Id, candidate.Id)
            });
        }

        return result;
    }

    public GetClassForTrainersDto GetClassById(Guid id)
    {
        var @class = genericRepository.GetById<Class>(id)
                     ?? throw new NotFoundException("The following class with the specified Id was not found.");

        var training = genericRepository.GetById<Training>(@class.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var result = new GetClassForTrainersDto()
        {
            Id = @class.Id,
            Title = @class.Title,
            TrainingId = @class.TrainingId,
            Date = @class.Date.ToFormattedDate(),
            StartTime = @class.StartTime.ToFormattedTime(),
            ImageUrl = @class.ImageUrl,
            EndTime = @class.EndTime.ToFormattedTime(),
            AssignedTrainers = genericRepository.Get<ClassTrainer>(x => x.ClassId == @class.Id).Count(),
            Training = training.Title,
            Status = GetClassStatus(@class.Date, @class.StartTime, @class.EndTime)
        };

        return result;
    }

    public void InsertClass(CreateClassDto @class)
    {
        var training = genericRepository.GetById<Training>(@class.TrainingId)
                       ?? throw new Exception("The respective training could not be found");

        var trainingClasses = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();

        if (DateOnly.FromDateTime(@class.Date) < training.StartDate ||
            DateOnly.FromDateTime(@class.Date) > training.EndDate)
        {
            throw new BadRequestException(
                "The respective class could not be registered.",
                ["Class dates should align with the training start and end dates."]);
        }

        if (@class.Date < DateTime.UtcNow.Date)
        {
            throw new BadRequestException(
                "The respective class could not be registered.",
                ["Class date cannot be in the past."]);
        }

        if (@class.StartTime >= @class.EndTime)
        {
            throw new BadRequestException(
                "The respective class could not be registered.",
                ["Class start time should be earlier than the end time."]);
        }

        if (trainingClasses.Any(c => c.Date == DateOnly.FromDateTime(@class.Date) &&
                                     ((c.StartTime <= @class.StartTime && c.EndTime > @class.StartTime) ||
                                      (c.StartTime < @class.EndTime && c.EndTime >= @class.EndTime) ||
                                      (c.StartTime >= @class.StartTime && c.EndTime <= @class.EndTime))))
        {
            throw new BadRequestException(
                "The respective class could not be registered.",
                ["An existing class is registered on the same date and time period."]);
        }

        var imageUrl = @class.Image != null
            ? fileService.UploadDocument(@class.Image, ClassesFilePath)
            : null;

        var classModel = new Class()
        {
            TrainingId = training.Id,
            Title = @class.Title,
            Date = DateOnly.FromDateTime(@class.Date),
            StartTime = @class.StartTime,
            EndTime = @class.EndTime,
            ImageUrl = imageUrl
        };

        var classId = genericRepository.Insert(classModel);

        var classResourceConfiguration = new ClassResourceConfiguration()
        {
            Accessibility = new AbstractClassResourceConfigurationDto()
            {
                AccessPeriod = training.StartDate.AddDays(-1).ToDateTime(TimeOnly.MinValue),
                ExpirePeriod = training.EndDate.AddMonths(6).ToDateTime(TimeOnly.MinValue)
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

        classConfigurationService.SaveProperty(classId, ClassConfiguration.RESOURCE_AVAILABILITY.ToString(),
            classResourceConfiguration);
        classConfigurationService.SaveProperty(classId, ClassConfiguration.ATTENDANCE_PERIOD.ToString(),
            classAttendanceConfiguration);
    }

    public void UpdateClass(UpdateClassDto @class)
    {
        var classModel = genericRepository.GetById<Class>(@class.Id) ??
                         throw new NotFoundException("The following class could not be found.");

        classModel.Title = @class.Title;
        classModel.EndTime = @class.EndTime;
        classModel.StartTime = @class.StartTime;
        classModel.Date = DateOnly.FromDateTime(@class.Date);

        if (@class.Image != null)
        {

            if (!string.IsNullOrEmpty(classModel.ImageUrl))
            {
                var classPath = Path.Combine(ClassesFilePath, classModel.ImageUrl);

                fileService.DeleteFile(classPath);    
            }
            
            classModel.ImageUrl = fileService.UploadDocument(@class.Image, ClassesFilePath);
        }

        genericRepository.Update(classModel);

        var training = genericRepository.GetById<Training>(classModel.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var classResourceConfiguration = new ClassResourceConfiguration()
        {
            Accessibility = new AbstractClassResourceConfigurationDto()
            {
                AccessPeriod = training.StartDate.AddDays(-1).ToDateTime(TimeOnly.MinValue),
                ExpirePeriod = training.EndDate.AddMonths(6).ToDateTime(TimeOnly.MinValue)
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

        classConfigurationService.SaveProperty(classModel.Id, ClassConfiguration.RESOURCE_AVAILABILITY.ToString(),
            classResourceConfiguration);
        classConfigurationService.SaveProperty(classModel.Id, ClassConfiguration.ATTENDANCE_PERIOD.ToString(),
            classAttendanceConfiguration);
    }

    public void DeleteClass(Guid id)
    {
        var classModel = genericRepository.GetById<Class>(id)
                         ?? throw new NotFoundException("The following class with the specified Id was not found.");

        genericRepository.Delete(classModel);
    }

    private static string GetClassStatus(DateOnly classDate, TimeSpan startTime, TimeSpan endTime)
    {
        var dateTime = DateTime.Now;

        var classDateTime = classDate.ToDateTime(new TimeOnly(endTime.Ticks));

        var timeSpan = DateTime.Now.TimeOfDay;
        
        if (dateTime >= classDateTime)
        {
            return Constants.Schedule.CompletedAction;
        }

        if (DateOnly.FromDateTime(dateTime) == classDate && timeSpan > startTime && timeSpan < endTime)
        {
            return Constants.Schedule.InProgressAction;
        }

        return Constants.Schedule.ScheduledAction;
    }

    private int GetAttendanceCount(Guid classId, int requestAction)
    {
        var attendances = genericRepository.Get<Attendance>(x => x.ClassId == classId).ToList();

        return requestAction switch
        {
            Constants.RequestAction.Pending => attendances.Count(x =>
                x is { IsApproved: false, IsActionCompleted: false }),
            Constants.RequestAction.Accepted => attendances.Count(x =>
                x is { IsApproved: true, IsActionCompleted: true }),
            Constants.RequestAction.Rejected => attendances.Count(x =>
                x is { IsApproved: false, IsActionCompleted: true }),
            _ => attendances.Count
        };
    }

    private Guid? GetAttendanceStatus(Guid classId, Guid candidateId)
    {
        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var attendance =
            genericRepository.GetFirstOrDefault<Attendance>(x => x.CandidateId == candidate.Id && x.ClassId == classId);

        return attendance?.Id;
    }

    private string GetAttendanceMarkedStatus(Guid classId, Guid candidateId)
    {
        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var attendance =
            genericRepository.GetFirstOrDefault<Attendance>(x => x.CandidateId == candidate.Id && x.ClassId == classId);

        return attendance != null ? Constants.RequestAction.MarkedAction : Constants.RequestAction.NotMarkedAction;
    }

    private string GetAttendanceApprovalStatus(Guid classId, Guid candidateId)
    {
        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var attendance =
            genericRepository.GetFirstOrDefault<Attendance>(x => x.CandidateId == candidate.Id && x.ClassId == classId);

        return attendance switch
        {
            { IsActionCompleted: true, IsApproved: true } => Constants.RequestAction.AcceptedAction,
            { IsActionCompleted: true, IsApproved: false } => Constants.RequestAction.RejectedAction,
            null => Constants.RequestAction.NotMarkedAction,
            _ => Constants.RequestAction.PendingAction
        };
    }

    public ClassCountDto GetClassDetailsCountForCandidate(Guid classId)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The respective class could not be found.");

        var attendanceCount = genericRepository.GetCount<Attendance>(x =>
            x.ClassId == @class.Id && x.CandidateId == candidate.Id);

        return new ClassCountDto()
        {
            AttendanceCount = attendanceCount,
            TrainersCount = GetTrainersCount(@class.Id),
            ResourceMaterialCount = GetResourceMaterialsCount(@class.Id, true)
        };
    }

    public ClassCountDto GetClassDetailsCountForClient(Guid classId)
    {
        var clientId = userService.GetUserId;

        var client = genericRepository.GetById<User>(clientId)
                     ?? throw new NotFoundException("The following client user has not been registered to our system.");

        var organizationalCandidates = genericRepository.Get<User>(x =>
            x.OrganizationId == client.OrganizationId).ToList();

        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The respective class could not be found.");

        var attendanceCount = genericRepository.GetCount<Attendance>(x =>
            x.ClassId == @class.Id && organizationalCandidates.Select(z => z.Id).Contains(x.CandidateId));

        return new ClassCountDto
        {
            AttendanceCount = attendanceCount,
            TrainersCount = GetTrainersCount(@class.Id),
            ResourceMaterialCount = GetResourceMaterialsCount(@class.Id, true)
        };
    }

    public ClassCountDto GetClassDetailsCountForTrainer(Guid classId)
    {
        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The respective class could not be found.");

        return new ClassCountDto
        {
            AttendanceCount = GetAttendanceCount(@class.Id),
            TrainersCount = GetTrainersCount(@class.Id),
            ResourceMaterialCount = GetResourceMaterialsCount(@class.Id)
        };
    }

    public ClassCountDto GetClassDetailsCountForAdmin(Guid classId)
    {
        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The respective class could not be found.");

        return new ClassCountDto
        {
            AttendanceCount = GetAttendanceCount(@class.Id),
            TrainersCount = GetTrainersCount(@class.Id),
            ResourceMaterialCount = GetResourceMaterialsCount(@class.Id)
        };
    }

    private int GetAttendanceCount(Guid classId)
    {
        return genericRepository.GetCount<Attendance>(x => x.ClassId == classId);
    }

    private int GetResourceMaterialsCount(Guid classId, bool? isActive = null)
    {
        return genericRepository.GetCount<ClassResources>(x => x.ClassId == classId && (isActive == null || x.IsActive));
    }

    private int GetTrainersCount(Guid classId)
    {
        return genericRepository.GetCount<ClassTrainer>(x => x.ClassId == classId);
    }
}