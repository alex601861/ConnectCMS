using CMSTrain.Helper;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Application.DTOs.Count;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Common.User;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Training;
using CMSTrain.Application.DTOs.ClassTrainers;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class ClassTrainersService(IGenericRepository genericRepository, ICurrentUserService userService)
    : IClassTrainersService
{
    public List<GetTrainersDto> GetAllActiveTrainers(int pageNumber, int pageSize, out int rowCount)
    {
        var trainerRole =
            genericRepository.Get<Role>(x =>
                x.Name == Constants.Roles.Trainer).ToList();

        var trainerUserIds =
            genericRepository.GetPagedResult<UserRoles>(pageNumber, pageSize, out rowCount,
                x => trainerRole.Select(z => z.Id).Contains(x.RoleId)).ToList();

        var users =
            genericRepository.Get<User>(x =>
                trainerUserIds.Select(z => z.UserId).Contains(x.Id) && x.IsActive);

        return users.Select(x => new GetTrainersDto()
        {
            Id = x.Id,
            EmailAddress = x.Email ?? "",
            PhoneNumber = x.PhoneNumber ?? "",
            Name = x.Name,
            Username = x.UserName ?? "",
            ImageUrl = x.ImageURL
        }).ToList();
    }

    public List<GetTrainersDto> GetAllActiveTrainers()
    {
        var trainerRole =
            genericRepository.GetFirstOrDefault<Role>(x =>
                x.Name == Constants.Roles.Trainer)
            ?? throw new NotFoundException("The following role has not been registered to our system.");

        var trainerUserIds =
            genericRepository.Get<UserRoles>(x =>
                x.RoleId == trainerRole.Id).ToList();

        var users =
            genericRepository.Get<User>(x =>
                trainerUserIds.Select(z => z.UserId).Contains(x.Id) && x.IsActive);

        return users.Select(x => new GetTrainersDto()
        {
            Id = x.Id,
            EmailAddress = x.Email ?? "",
            PhoneNumber = x.PhoneNumber ?? "",
            Name = x.Name,
            Username = x.UserName ?? "",
            ImageUrl = x.ImageURL
        }).ToList();
    }

    public List<GetTrainingDto> GetAllAvailableTrainingsForTrainers(int pageNumber, int pageSize, out int rowCount,
        string? search = null)
    {
        var trainerId = userService.GetUserId;

        var trainer = genericRepository.GetById<User>(trainerId) ??
                      throw new NotFoundException("The following trainer has not been registered to our system");

        var assignedClasses = genericRepository.Get<ClassTrainer>(x =>
            x.TrainerId == trainer.Id).ToList();

        var classes = genericRepository.Get<Class>(x =>
            assignedClasses.Select(z => z.ClassId).Contains(x.Id)).ToList();

        var trainings = genericRepository.GetPagedResult<Training>(pageNumber, pageSize, out rowCount,
            x =>
                !classes.Select(z => z.TrainingId).Contains(x.Id) &&
                x.EndDate >= DateOnly.FromDateTime(DateTime.Now) &&
                (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())) && x.IsActive).ToList();

        return trainings.Select(x => new GetTrainingDto
        {
            Id = x.Id,
            Description = x.Description,
            ImageUrl = x.ImageUrl,
            Title = x.Title,
            LocationDetails = x.LocationDetails,
            Latitude = x.Latitude ?? 0m,
            Longitude = x.Longitude ?? 0m,
            StartDate = x.StartDate.ToFormattedDate(),
            EndDate = x.EndDate.ToFormattedDate(),
            TrainingFormatId = x.TrainingFormatId,
            TrainingFormat = genericRepository.GetById<TrainingFormat>(x.TrainingFormatId)!.Name,
            IsActive = x.IsActive,
            AssignedTrainers = GetAssignedTrainingsTrainers(x.Id)
        }).ToList();
    }

    public List<GetTrainingDto> GetAllAvailableTrainingsForTrainers(string? search = null)
    {
        var trainerId = userService.GetUserId;

        var trainer = genericRepository.GetById<User>(trainerId) ??
                      throw new NotFoundException("The following trainer has not been registered to our system");

        var assignedClasses = genericRepository.Get<ClassTrainer>(x =>
            x.TrainerId == trainer.Id).ToList();

        var classes = genericRepository.Get<Class>(x =>
            assignedClasses.Select(z => z.ClassId).Contains(x.Id)).ToList();

        var trainings = genericRepository.Get<Training>(x =>
            !classes.Select(z => z.TrainingId).Contains(x.Id) &&
            x.EndDate >= DateOnly.FromDateTime(DateTime.Now) &&
            (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())) && x.IsActive).ToList();

        return trainings.Select(x => new GetTrainingDto()
        {
            Id = x.Id,
            Description = x.Description,
            ImageUrl = x.ImageUrl,
            Title = x.Title,
            LocationDetails = x.LocationDetails,
            Latitude = x.Latitude ?? 0m,
            Longitude = x.Longitude ?? 0m,
            StartDate = x.StartDate.ToFormattedDate(),
            EndDate = x.EndDate.ToFormattedDate(),
            TrainingFormatId = x.TrainingFormatId,
            TrainingFormat = genericRepository.GetById<TrainingFormat>(x.TrainingFormatId)!.Name,
            IsActive = x.IsActive,
            AssignedTrainers = GetAssignedTrainingsTrainers(x.Id)
        }).ToList();
    }

    public AvailableTrainingCountDto GetAllAvailableTrainingCountForTrainers()
    {
        var trainerId = userService.GetUserId;

        var trainer = genericRepository.GetById<User>(trainerId) ??
                      throw new NotFoundException("The following trainer has not been registered to our system");

        var assignedClasses = genericRepository.Get<ClassTrainer>(x =>
            x.TrainerId == trainer.Id).ToList();

        var classes = genericRepository.Get<Class>(x =>
            assignedClasses.Select(z => z.ClassId).Contains(x.Id)).ToList();

        return new AvailableTrainingCountDto()
        {
            AvailableCount = genericRepository.GetCount<Training>(x =>
                !classes.Select(z =>
                    z.TrainingId).Contains(x.Id) && x.EndDate >= DateOnly.FromDateTime(DateTime.Now) && x.IsActive)
        };
    }

    public List<GetAssignedTrainingsDto> GetAllAssignedTrainingsForTrainers(int statusAction, int pageNumber,
        int pageSize, out int rowCount, string? search = null)
    {
        var trainerId = userService.GetUserId;

        var trainer = genericRepository.GetById<User>(trainerId) ??
                      throw new NotFoundException("The following trainer has not been registered to our system");

        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        var classTrainerQuery = from ct in genericRepository.Get<ClassTrainer>()
                                join cls in genericRepository.Get<Class>() on ct.ClassId equals cls.Id
                                join training in genericRepository.Get<Training>() on cls.TrainingId equals training.Id
                                where ct.TrainerId == trainer.Id && training.IsActive
                                select new
                                {
                                    Training = training,
                                    Class = cls
                                };

        classTrainerQuery = statusAction switch
        {
            Constants.StatusAction.Available => classTrainerQuery.Where(x =>
                x.Training.EndDate >= currentDate &&
                (string.IsNullOrEmpty(search) || x.Training.Title.ToLower().Contains(search.ToLower()))),
            Constants.StatusAction.Expired => classTrainerQuery.Where(x =>
                x.Training.EndDate < currentDate &&
                (string.IsNullOrEmpty(search) || x.Training.Title.ToLower().Contains(search.ToLower()))),
            Constants.StatusAction.All => classTrainerQuery.Where(x =>
                string.IsNullOrEmpty(search) || x.Training.Title.ToLower().Contains(search.ToLower())),
            _ => throw new ArgumentOutOfRangeException(nameof(statusAction), statusAction, null)
        };

        var groupedTrainings = classTrainerQuery
            .GroupBy(x => x.Training.Id)
            .Select(g => new
            {
                TrainingId = g.Key,
                ClassCount = g.Count(),
                NextClass = g
                    .Where(x => x.Class.Date >= DateOnly.FromDateTime(DateTime.UtcNow) &&
                                x.Class.StartTime > DateTime.Now.TimeOfDay)
                    .OrderBy(x => x.Class.Date)
                    .ThenBy(x => x.Class.StartTime)
                    .Select(x => x.Class)
                    .FirstOrDefault()
            });

        rowCount = groupedTrainings.Count();

        var pagedTrainingMeta = groupedTrainings
            .OrderBy(x => x.TrainingId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var trainingIds = pagedTrainingMeta.Select(x => x.TrainingId).ToList();

        var trainings = genericRepository.Get<Training>(x => trainingIds.Contains(x.Id)).ToList();
        var formats = genericRepository.Get<TrainingFormat>().ToList();

        var result = new List<GetAssignedTrainingsDto>();

        foreach (var meta in pagedTrainingMeta)
        {
            var training = trainings.First(t => t.Id == meta.TrainingId);

            var format = formats.FirstOrDefault(f => f.Id == training.TrainingFormatId)
                ?? throw new NotFoundException("Training format not found.");

            result.Add(new GetAssignedTrainingsDto
            {
                Id = training.Id,
                Description = training.Description,
                LocationDetails = training.LocationDetails,
                Latitude = training.Latitude ?? 0m,
                Longitude = training.Longitude ?? 0m,
                StartDate = training.StartDate.ToFormattedDate(),
                EndDate = training.EndDate.ToFormattedDate(),
                ImageUrl = training.ImageUrl,
                TrainingFormatId = training.TrainingFormatId,
                TrainingFormat = format.Name,
                IsActive = training.IsActive,
                Title = training.Title,
                AssignedClasses = meta.ClassCount,
                NextClassDate = meta.NextClass != null
                    ? meta.NextClass.Date.ToFormattedDate()
                    : "No Upcoming Classes",
                AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
            });
        }

        return result;
    }

    public List<GetAssignedTrainingsDto> GetAllAssignedTrainingsForTrainers(int statusAction, string? search = null)
    {
        var trainerId = userService.GetUserId;

        var trainer = genericRepository.GetById<User>(trainerId) ??
                      throw new NotFoundException("The following trainer has not been registered to our system");

        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        var classTrainerQuery = from ct in genericRepository.Get<ClassTrainer>()
                                join cls in genericRepository.Get<Class>() on ct.ClassId equals cls.Id
                                join training in genericRepository.Get<Training>() on cls.TrainingId equals training.Id
                                where ct.TrainerId == trainer.Id && training.IsActive
                                select new
                                {
                                    Training = training,
                                    Class = cls
                                };

        classTrainerQuery = statusAction switch
        {
            Constants.StatusAction.Available => classTrainerQuery.Where(x =>
                x.Training.EndDate >= currentDate &&
                (string.IsNullOrEmpty(search) || x.Training.Title.ToLower().Contains(search.ToLower()))),
            Constants.StatusAction.Expired => classTrainerQuery.Where(x =>
                x.Training.EndDate < currentDate &&
                (string.IsNullOrEmpty(search) || x.Training.Title.ToLower().Contains(search.ToLower()))),
            Constants.StatusAction.All => classTrainerQuery.Where(x =>
                string.IsNullOrEmpty(search) || x.Training.Title.ToLower().Contains(search.ToLower())),
            _ => throw new ArgumentOutOfRangeException(nameof(statusAction), statusAction, null)
        };

        var groupedTrainings = classTrainerQuery
            .GroupBy(x => x.Training.Id)
            .Select(g => new
            {
                TrainingId = g.Key,
                ClassCount = g.Count(),
                NextClass = g
                    .Where(x => x.Class.Date >= DateOnly.FromDateTime(DateTime.UtcNow) &&
                                x.Class.StartTime > DateTime.Now.TimeOfDay)
                    .OrderBy(x => x.Class.Date)
                    .ThenBy(x => x.Class.StartTime)
                    .Select(x => x.Class)
                    .FirstOrDefault()
            });

        var pagedTrainingMeta = groupedTrainings
            .OrderBy(x => x.TrainingId)
            .ToList();

        var trainingIds = pagedTrainingMeta.Select(x => x.TrainingId).ToList();

        var trainings = genericRepository.Get<Training>(x => trainingIds.Contains(x.Id)).ToList();
        var formats = genericRepository.Get<TrainingFormat>().ToList();

        var result = new List<GetAssignedTrainingsDto>();

        foreach (var meta in pagedTrainingMeta)
        {
            var training = trainings.First(t => t.Id == meta.TrainingId);

            var format = formats.FirstOrDefault(f => f.Id == training.TrainingFormatId)
                ?? throw new NotFoundException("Training format not found.");

            result.Add(new GetAssignedTrainingsDto
            {
                Id = training.Id,
                Description = training.Description,
                LocationDetails = training.LocationDetails,
                Latitude = training.Latitude ?? 0m,
                Longitude = training.Longitude ?? 0m,
                StartDate = training.StartDate.ToFormattedDate(),
                EndDate = training.EndDate.ToFormattedDate(),
                ImageUrl = training.ImageUrl,
                TrainingFormatId = training.TrainingFormatId,
                TrainingFormat = format.Name,
                IsActive = training.IsActive,
                Title = training.Title,
                AssignedClasses = meta.ClassCount,
                NextClassDate = meta.NextClass != null
                    ? meta.NextClass.Date.ToFormattedDate()
                    : "No Upcoming Classes",
                AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
            });
        }

        return result;
    }

    public AssignedTrainingCountDto GetAllAssignedTrainingCountForTrainers()
    {
        var trainerId = userService.GetUserId;

        var trainer = genericRepository.GetById<User>(trainerId) ??
                      throw new NotFoundException("The following trainer has not been registered to our system");

        var assignedClasses = genericRepository.Get<ClassTrainer>(x =>
            x.TrainerId == trainer.Id).ToList();

        var classes = genericRepository.Get<Class>(x =>
            assignedClasses.Select(z => z.ClassId).Contains(x.Id)).ToList();

        return new AssignedTrainingCountDto()
        {
            AllCount = genericRepository.GetCount<Training>(x =>
                classes.Select(z => z.TrainingId).Contains(x.Id) && x.IsActive),
            AvailableCount = genericRepository.GetCount<Training>(x =>
                classes.Select(z => z.TrainingId).Contains(x.Id) && x.EndDate >= DateOnly.FromDateTime(DateTime.Now) && x.IsActive),
            ExpiredCount = genericRepository.GetCount<Training>(x =>
                classes.Select(z => z.TrainingId).Contains(x.Id) && x.EndDate < DateOnly.FromDateTime(DateTime.Now) && x.IsActive)
        };
    }

    public List<GetAssignedTrainersDto> GetAllTrainersForTraining(Guid trainingId, int pageNumber, int pageSize,
        out int rowCount, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The respective training could not be found.");

        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();

        var classTrainers = genericRepository
            .GetPagedResult<ClassTrainer>(pageNumber, pageSize, out rowCount,
                x => classes.Select(z => z.Id).Contains(x.ClassId)).ToList().DistinctBy(x => x.TrainerId);

        var result = new List<GetAssignedTrainersDto>();

        foreach (var classTrainer in classTrainers)
        {
            var trainer = genericRepository.GetById<User>(classTrainer.TrainerId)
                          ?? throw new NotFoundException(
                              "The following trainer has not been registered to our system.");

            if (!string.IsNullOrEmpty(search) && 
                (!trainer.Name.ToLower().Contains(search.ToLower()) || 
                (trainer.Email != null && !trainer.Email.ToLower().Contains(search.ToLower())) || 
                (trainer.PhoneNumber != null && !trainer.PhoneNumber.ToLower() .Contains(search.ToLower()))))
            {
                continue;
            }

            result.Add(new GetAssignedTrainersDto()
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Username = trainer.UserName ?? "",
                EmailAddress = trainer.Email ?? "",
                ImageUrl = trainer.ImageURL,
                PhoneNumber = trainer.PhoneNumber ?? "",
                AssignedBy = trainer.Name,
                Description = classTrainer.Description ?? "",
                AssignedDate = classTrainer.CreatedAt.ToFormattedDateTime()
            });
        }

        return result;
    }

    public List<GetAssignedTrainersDto> GetAllTrainersForTraining(Guid trainingId, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The respective training could not be found");

        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();

        var classTrainers =
            genericRepository.Get<ClassTrainer>(x =>
                classes.Select(z => z.Id).Contains(x.ClassId)).DistinctBy(x => x.TrainerId).ToList();

        var result = new List<GetAssignedTrainersDto>();

        foreach (var classTrainer in classTrainers)
        {
            var trainer = genericRepository.GetById<User>(classTrainer.TrainerId)
                          ?? throw new NotFoundException(
                              "The following trainer has not been registered to our system.");

            result.Add(new GetAssignedTrainersDto()
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Username = trainer.UserName ?? "",
                EmailAddress = trainer.Email ?? "",
                ImageUrl = trainer.ImageURL,
                PhoneNumber = trainer.PhoneNumber ?? "",
                AssignedBy = trainer.Name,
                Description = classTrainer.Description ?? "",
                AssignedDate = classTrainer.CreatedAt.ToFormattedDateTime(),
            });
        }

        return result;
    }

    public List<GetAssignedTrainersDto> GetAllTrainersForClass(Guid classId, int pageNumber, int pageSize,
        out int rowCount, string? search = null)
    {
        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The respective class could not be found.");

        var classTrainers =
            genericRepository.GetPagedResult<ClassTrainer>(pageNumber, pageSize, out rowCount,
                x => x.ClassId == @class.Id).ToList();

        var result = new List<GetAssignedTrainersDto>();

        foreach (var classTrainer in classTrainers)
        {
            var trainer = genericRepository.GetById<User>(classTrainer.TrainerId)
                          ?? throw new NotFoundException(
                              "The following trainer has not been registered to our system.");

            if (!string.IsNullOrEmpty(search) && (!trainer.Name.ToLower().Contains(search.ToLower()) ||
                                                  (trainer.Email != null &&
                                                   !trainer.Email.ToLower().Contains(search.ToLower())) ||
                                                  (trainer.PhoneNumber != null && !trainer.PhoneNumber.ToLower()
                                                      .Contains(search.ToLower()))))
            {
                continue;
            }

            result.Add(new GetAssignedTrainersDto()
            {
                Id = trainer.Id,
                ClassTrainerId = classTrainer.Id,
                Name = trainer.Name,
                Username = trainer.UserName ?? "",
                EmailAddress = trainer.Email ?? "",
                ImageUrl = trainer.ImageURL,
                PhoneNumber = trainer.PhoneNumber ?? "",
                AssignedBy = trainer.Name,
                Description = classTrainer.Description ?? "",
                AssignedDate = classTrainer.CreatedAt.ToFormattedDateTime()
            });
        }

        return result;
    }

    public List<GetAssignedTrainersDto> GetAllTrainersForClass(Guid classId, string? search = null)
    {
        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The respective class could not be found.");

        var classTrainers =
            genericRepository.Get<ClassTrainer>(x =>
                x.ClassId == @class.Id).ToList();

        var result = new List<GetAssignedTrainersDto>();

        foreach (var classTrainer in classTrainers)
        {
            var trainer = genericRepository.GetById<User>(classTrainer.TrainerId)
                          ?? throw new NotFoundException(
                              "The following trainer has not been registered to our system.");

            result.Add(new GetAssignedTrainersDto()
            {
                Id = trainer.Id,
                ClassTrainerId = classTrainer.Id,
                Name = trainer.Name,
                Username = trainer.UserName ?? "",
                EmailAddress = trainer.Email ?? "",
                ImageUrl = trainer.ImageURL,
                PhoneNumber = trainer.PhoneNumber ?? "",
                AssignedBy = trainer.Name,
                Description = classTrainer.Description ?? "",
                AssignedDate = classTrainer.CreatedAt.ToFormattedDateTime()
            });
        }

        return result;
    }

    public void AssignTrainersToClass(AssignTrainersDto trainingAssignment)
    {
        var @class = genericRepository.GetById<Class>(trainingAssignment.ClassId)
                     ?? throw new NotFoundException("The respective class could not be found.");

        var assignedTrainers = genericRepository.Get<ClassTrainer>(t => t.ClassId == @class.Id).ToList();

        if (assignedTrainers.Any())
        {
            genericRepository.RemoveMultipleEntity(assignedTrainers);
        }

        var trainingTrainerModel = trainingAssignment.TrainerIds.Select(trainerId => new ClassTrainer()
        {
            ClassId = @class.Id,
            TrainerId = trainerId,
        }).ToList();

        genericRepository.AddMultipleEntity(trainingTrainerModel);
    }

    public void UpdateTrainerDescription(Guid classTrainerId, UpdateClassTrainerDescriptionDto classTrainerDescription)
    {
        if (classTrainerDescription.ClassTrainerId != classTrainerId)
        {
            throw new BadRequestException("The respective class trainer's identifier does not match with the provided identifier.", []);
        }
        
        var classTrainer = genericRepository.GetById<ClassTrainer>(classTrainerDescription.ClassTrainerId)
                           ?? throw new NotFoundException("The following trainer has not been assigned to the respective class.");
        
        classTrainer.Description = classTrainerDescription.Description;

        genericRepository.Update(classTrainer);
    }

    public GetTrainerDescriptionDto GetTrainerDescriptionsOnTraining(Guid trainingId, Guid trainerId)
    {
        var trainer = genericRepository.GetById<User>(trainerId)
                      ?? throw new NotFoundException(
                          "The following trainer has not been registered to our system.");

        var trainerModel = new GetTrainersDto()
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Username = trainer.UserName ?? "",
            EmailAddress = trainer.Email ?? "",
            ImageUrl = trainer.ImageURL,
            PhoneNumber = trainer.PhoneNumber ?? ""
        };
        
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training was not found.");
        
        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();
        
        var classTrainers = genericRepository.Get<ClassTrainer>(x => 
                x.TrainerId == trainer.Id && classes.Select(z => z.Id).Contains(x.ClassId)) .ToList();

        var classesModels = new List<GetClassTrainerDescriptionDto>();
        
        foreach (var classTrainer in classTrainers)
        {
            var @class = classes.FirstOrDefault(x => x.Id == classTrainer.ClassId)
                         ?? throw new NotFoundException("The respective class could not be found.");
            
            classesModels.Add(new GetClassTrainerDescriptionDto()
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
                ImageUrl = @class.ImageUrl,
                Description = classTrainer.Description ?? ""
            });
        }

        return new GetTrainerDescriptionDto()
        {
            Trainer = trainerModel,
            Classes = classesModels
        };
    }

    public GetTrainerDescriptionDto GetTrainerDescriptionsOnClass(Guid classId, Guid trainerId)
    {
        var trainer = genericRepository.GetById<User>(trainerId)
                      ?? throw new NotFoundException(
                          "The following trainer has not been registered to our system.");

        var trainerModel = new GetTrainersDto()
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Username = trainer.UserName ?? "",
            EmailAddress = trainer.Email ?? "",
            ImageUrl = trainer.ImageURL,
            PhoneNumber = trainer.PhoneNumber ?? ""
        };
        
        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The respective class could not be found.");
        
        var classTrainer = genericRepository.GetFirstOrDefault<ClassTrainer>(x =>
            x.TrainerId == trainer.Id && x.ClassId == @class.Id);

        var classDetails = new GetClassTrainerDescriptionDto()
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
            ImageUrl = @class.ImageUrl,
            Description = classTrainer?.Description ?? ""
        };
            
        return new GetTrainerDescriptionDto()
        {
            Trainer = trainerModel,
            Classes = [classDetails]
        };
    }
    private GetAssignedTrainingsTrainersDto GetAssignedTrainingsTrainers(Guid trainingId)
    {
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training was not found.");

        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();

        var classTrainers = genericRepository.Get<ClassTrainer>(x => classes.Select(z => z.Id).Contains(x.ClassId))
            .ToList();

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
}