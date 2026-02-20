using CMSTrain.Helper;
using System.Globalization;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Common.User;
using CMSTrain.Domain.Entities.Identity; 
using CMSTrain.Application.DTOs.Training;
using CMSTrain.Application.DTOs.Dashboard;
using CMSTrain.Application.DTOs.ClassTrainers;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class DashboardService(IGenericRepository genericRepository, ICurrentUserService userService) : IDashboardService
{
    #region Admin
    public GetAdminCountDto GetAdminDashboardCount(int period)
    {
        var today = DateTime.UtcNow;
        
        var currentWeekStart = today.AddDays(-(int)today.DayOfWeek);  
        var currentWeekEnd = currentWeekStart.AddDays(6);       
        
        var previousWeekStart = currentWeekStart.AddDays(-7);         
        var previousWeekEnd = currentWeekStart.AddDays(-1);  
        
        var currentMonthStart = new DateTime(today.Year, today.Month, 1).ToUniversalTime();
        var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1); 
        
        var previousMonthStart = currentMonthStart.AddMonths(-1).ToUniversalTime(); 
        var previousMonthEnd = currentMonthStart.AddDays(-1);   
        
        var currentYearStart = new DateTime(today.Year, 1, 1).ToUniversalTime();
        var currentYearEnd = new DateTime(today.Year, 12, 31).ToUniversalTime();
        
        var previousYearStart = new DateTime(today.Year - 1, 1, 1).ToUniversalTime();
        var previousYearEnd = new DateTime(today.Year - 1, 12, 31).ToUniversalTime();
        
        var trainings = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Training>(),
                PreviousPeriodCount = genericRepository.GetCount<Training>()
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Training>(x => x.StartDate >= DateOnly.FromDateTime(currentWeekStart) && x.StartDate <= DateOnly.FromDateTime(currentWeekEnd)),
                PreviousPeriodCount = genericRepository.GetCount<Training>(x => x.StartDate >= DateOnly.FromDateTime(previousWeekStart) && x.StartDate <= DateOnly.FromDateTime(previousWeekEnd))
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Training>(x => x.StartDate >= DateOnly.FromDateTime(currentMonthStart) && x.StartDate <= DateOnly.FromDateTime(currentMonthEnd)),
                PreviousPeriodCount = genericRepository.GetCount<Training>(x => x.StartDate >= DateOnly.FromDateTime(previousMonthStart) && x.StartDate <= DateOnly.FromDateTime(previousMonthEnd))
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Training>(x => x.StartDate >= DateOnly.FromDateTime(currentYearStart) && x.StartDate <= DateOnly.FromDateTime(currentYearEnd)),
                PreviousPeriodCount = genericRepository.GetCount<Training>(x => x.StartDate >= DateOnly.FromDateTime(previousYearStart) && x.StartDate <= DateOnly.FromDateTime(previousYearEnd))
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };

        var candidateRole = genericRepository.GetFirstOrDefault<Role>(r => r.Name == Constants.Roles.Candidate)
            ?? throw new NotFoundException("The following candidate role has not been registered to the system.");

        var candidateUserRoles = genericRepository.Get<UserRoles>(ur => ur.RoleId == candidateRole.Id).ToList();
        
        var candidates = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount =  genericRepository.GetCount<User>(x => candidateUserRoles.Select(z => z.UserId).Contains(x.Id)),
                PreviousPeriodCount = genericRepository.GetCount<User>(x => candidateUserRoles.Select(z => z.UserId).Contains(x.Id))
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<User>(x => candidateUserRoles.Select(z => z.UserId).Contains(x.Id) && x.RegisteredDate >= currentWeekStart && x.RegisteredDate <= currentWeekStart),
                PreviousPeriodCount = genericRepository.GetCount<User>(x => candidateUserRoles.Select(z => z.UserId).Contains(x.Id) && x.RegisteredDate >= previousWeekStart && x.RegisteredDate <= previousWeekEnd)
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<User>(x => candidateUserRoles.Select(z => z.UserId).Contains(x.Id) && x.RegisteredDate >= currentMonthStart && x.RegisteredDate <= currentMonthEnd),
                PreviousPeriodCount = genericRepository.GetCount<User>(x => candidateUserRoles.Select(z => z.UserId).Contains(x.Id) && x.RegisteredDate >= previousMonthStart && x.RegisteredDate <= previousMonthEnd)
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<User>(x => candidateUserRoles.Select(z => z.UserId).Contains(x.Id) && x.RegisteredDate >= currentYearStart && x.RegisteredDate <= currentYearEnd),
                PreviousPeriodCount = genericRepository.GetCount<User>(x => candidateUserRoles.Select(z => z.UserId).Contains(x.Id) && x.RegisteredDate >= previousYearStart && x.RegisteredDate <= previousYearEnd)
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };
        
        var trainerRole = genericRepository.GetFirstOrDefault<Role>(r => r.Name == Constants.Roles.Trainer)
                            ?? throw new NotFoundException("The following trainer role has not been registered to the system.");

        var trainerUserRoles = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = genericRepository.GetCount<UserRoles>(x => x.RoleId == trainerRole.Id),
                PreviousPeriodCount = genericRepository.GetCount<UserRoles>(x => x.RoleId == trainerRole.Id)
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<User>(x => x.Id == trainerRole.Id && x.RegisteredDate >= currentWeekStart && x.RegisteredDate <= currentWeekStart),
                PreviousPeriodCount = genericRepository.GetCount<User>(x => x.Id == trainerRole.Id && x.RegisteredDate >= previousWeekStart && x.RegisteredDate <= previousWeekEnd)
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<User>(x => x.Id == trainerRole.Id && x.RegisteredDate >= currentMonthStart && x.RegisteredDate <= currentMonthEnd),
                PreviousPeriodCount = genericRepository.GetCount<User>(x => x.Id == trainerRole.Id && x.RegisteredDate >= previousMonthStart && x.RegisteredDate <= previousMonthEnd)
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<User>(x => x.Id == trainerRole.Id && x.RegisteredDate >= currentYearStart && x.RegisteredDate <= currentYearEnd),
                PreviousPeriodCount = genericRepository.GetCount<User>(x => x.Id == trainerRole.Id && x.RegisteredDate >= previousYearStart && x.RegisteredDate <= previousYearEnd)
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };

        var trainingCandidateRequests = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = genericRepository.GetCount<TrainingCandidate>(x => !x.IsActionCompleted),
                PreviousPeriodCount = genericRepository.GetCount<TrainingCandidate>(x => !x.IsActionCompleted)
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<TrainingCandidate>(x => !x.IsActionCompleted && x.RequestedDate >= currentWeekStart && x.RequestedDate <= currentWeekStart),
                PreviousPeriodCount = genericRepository.GetCount<TrainingCandidate>(x => !x.IsActionCompleted && x.RequestedDate >= previousWeekStart && x.RequestedDate <= previousWeekEnd)
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<TrainingCandidate>(x => !x.IsActionCompleted && x.RequestedDate >= currentMonthStart && x.RequestedDate <= currentMonthEnd),
                PreviousPeriodCount = genericRepository.GetCount<TrainingCandidate>(x => !x.IsActionCompleted && x.RequestedDate >= previousMonthStart && x.RequestedDate <= previousMonthEnd)
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<TrainingCandidate>(x => !x.IsActionCompleted && x.RequestedDate >= currentYearStart && x.RequestedDate <= currentYearEnd),
                PreviousPeriodCount = genericRepository.GetCount<TrainingCandidate>(x => !x.IsActionCompleted && x.RequestedDate >= previousYearStart && x.RequestedDate <= previousYearEnd)
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };
        
        double trainingGrowthPercent = trainings.PreviousPeriodCount > 0 
            ? Math.Round(((double)(trainings.CurrentPeriodCount - trainings.PreviousPeriodCount) / trainings.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        double totalRegisteredCandidatesGrowth = candidates.PreviousPeriodCount > 0 
            ? Math.Round(((double)(candidates.CurrentPeriodCount - candidates.PreviousPeriodCount) / candidates.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        double totalRegisteredTrainersGrowth = trainerUserRoles.PreviousPeriodCount > 0 
            ? Math.Round(((double)(trainerUserRoles.CurrentPeriodCount - trainerUserRoles.PreviousPeriodCount) / trainerUserRoles.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        double trainingCandidateRequestsGrowth = trainingCandidateRequests.PreviousPeriodCount > 0 
            ? Math.Round(((double)(trainingCandidateRequests.CurrentPeriodCount - trainingCandidateRequests.PreviousPeriodCount) / trainingCandidateRequests.PreviousPeriodCount) * 100, 0)
            : 0; 

        return new GetAdminCountDto
        {
            TotalTrainings = trainings.CurrentPeriodCount,
            TrainingGrowthPercent = trainingGrowthPercent,
            TotalRegisteredCandidates = candidates.CurrentPeriodCount,
            TotalRegisteredCandidatesGrowth = totalRegisteredCandidatesGrowth,
            TotalRegisteredTrainers = trainerUserRoles.CurrentPeriodCount,
            TotalRegisteredTrainersGrowth = totalRegisteredTrainersGrowth,
            TotalPendingRequests = trainingCandidateRequests.CurrentPeriodCount, 
            TotalPendingRequestsGrowth = trainingCandidateRequestsGrowth
        };
    }

    public List<GetPopularTrainingDto> GetPopularTrainings()
    {
        var popularTrainings = genericRepository.Get<TrainingCandidate>()
            .GroupBy(tc => tc.TrainingId)
            .Select(group => new
            {
                TrainingId = group.Key,
                CandidateCount = group.Count()
            }).OrderByDescending(g => g.CandidateCount)
            .Take(3).ToList();

        var result = new List<GetPopularTrainingDto>();

        foreach(var popularTraining in popularTrainings)
        {
            var training = genericRepository.GetById<Training>(popularTraining.TrainingId)
                ?? throw new NotFoundException("The following training could not be found.");

            result.Add(new GetPopularTrainingDto()
            {
                Id = training.Id,
                Title = training.Title,
                Description = training.Description,
                TrainingFormatId = training.TrainingFormatId,
                TrainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)?.Name ?? throw new NotFoundException("The following training format could not be found."),
                ImageUrl = training.ImageUrl,
                Location = training.LocationDetails,
                Latitude = training.Latitude ?? 0m,
                Longitude = training.Longitude ?? 0m,
                Date = $"{training.StartDate.ToFormattedDate()} - {training.EndDate.ToFormattedDate()}",
                AcceptedRequests = genericRepository.Get<TrainingCandidate>(x => x.TrainingId == training.Id && x.IsApproved).ToList().Count,
                AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
            });
        }
        
        return result;
    }

    public List<GetUpcomingTrainingsDto> GetUpcomingTrainings()
    {
        var upcomingTrainings = genericRepository.Get<Training>(t => 
            t.StartDate > DateOnly.FromDateTime(DateTime.Now))
                .OrderBy(t => t.StartDate).ToList();

        var result = new List<GetUpcomingTrainingsDto>();

        foreach(var upcomingTraining in upcomingTrainings)
        {
            var training = genericRepository.GetById<Training>(upcomingTraining.Id)
                           ?? throw new NotFoundException("The following training could not be found.");

            var classes = genericRepository.Get<Class>(c => c.TrainingId == training.Id).ToList();

            var trainingCandidateRequests = genericRepository.Get<TrainingCandidate>(x => 
                x.TrainingId == training.Id).ToList();
            
            result.Add(new GetUpcomingTrainingsDto()
            {
                Id = training.Id,
                Title = training.Title,
                Description = training.Description,
                TrainingFormatId = training.TrainingFormatId,
                TrainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)?.Name ?? throw new NotFoundException("The following training format could not be found."),
                ImageUrl = training.ImageUrl,
                Location = training.LocationDetails,
                Latitude = training.Latitude ?? 0m,
                Longitude = training.Longitude ?? 0m,
                Date = $"{training.StartDate.ToFormattedDate()} - {training.EndDate.ToFormattedDate()}",
                AcceptedRequests = trainingCandidateRequests.Count(x => x.IsApproved),
                PendingRequests = trainingCandidateRequests.Count(x => !x.IsActionCompleted),
                Classes = classes.Count,
                AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
            });
        }
        
        return result;
    }
    
    public List<GetTrainingFormatCountDto> GetTotalTrainingFormats()
    {
        var trainingFormats = genericRepository.Get<Training>()
            .GroupBy(t => t.TrainingFormatId)
            .Select(group => new
            {
                Id = group.Key,   
                TotalCount = group.Count()
            }).ToList();

        var result = trainingFormats.Select(x => new GetTrainingFormatCountDto
        {
            Id = x.Id,
            Name = genericRepository.GetById<TrainingFormat>(x.Id)?.Name ?? throw new NotFoundException("The following training format could not be found."),
            TotalCount = x.TotalCount
        }).ToList();

        return result;
    }

    public GetTrainingRequestsSummaryDto GetTrainingRequestSummary(int year)
    {
        var trainings = genericRepository.Get<Training>(x => x.StartDate.Year == year).ToList();

        var firstTrainings = new List<TrainingStartDateSummary>();
        var secondTrainings = new List<TrainingStartDateSummary>();
        var thirdTrainings = new List<TrainingStartDateSummary>();
        var fourthTrainings = new List<TrainingStartDateSummary>();
        var fifthTrainings = new List<TrainingStartDateSummary>();

        for (var i = 1; i <= 12; i++)
        {
            var monthlyTrainings = trainings.Where(x => x.StartDate.Month == i).ToList();
            var trainingLists = new[] { firstTrainings, secondTrainings, thirdTrainings, fourthTrainings, fifthTrainings };

            for (var j = 0; j < trainingLists.Length; j++)
            {
                if (j < monthlyTrainings.Count)
                {
                    trainingLists[j].Add(new TrainingStartDateSummary
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i),
                        Date = monthlyTrainings[j].StartDate.Day,
                        Title = monthlyTrainings[j].Title
                    });
                }
                else
                {
                    trainingLists[j].Add(new TrainingStartDateSummary
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i),
                        Date = null,
                        Title = "Training"
                    });
                }
            }
        }

        return new GetTrainingRequestsSummaryDto
        {
            FirstTraining = firstTrainings,
            SecondTraining = secondTrainings,
            ThirdTraining = thirdTrainings,
            FourthTraining = fourthTrainings,
            FifthTraining = fifthTrainings
        };
    }
    #endregion

    #region Trainers
    public GetDashboardCount GetTrainerDashboardCount(int period)
    {
        var today = DateTime.UtcNow;
        
        var currentWeekStart = today.AddDays(-(int)today.DayOfWeek);  
        var currentWeekEnd = currentWeekStart.AddDays(6);       
        
        var previousWeekStart = currentWeekStart.AddDays(-7);         
        var previousWeekEnd = currentWeekStart.AddDays(-1);  
        
        var currentMonthStart = new DateTime(today.Year, today.Month, 1).ToUniversalTime();
        var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1); 
        
        var previousMonthStart = currentMonthStart.AddMonths(-1).ToUniversalTime(); 
        var previousMonthEnd = currentMonthStart.AddDays(-1);   
        
        var currentYearStart = new DateTime(today.Year, 1, 1).ToUniversalTime();
        var currentYearEnd = new DateTime(today.Year, 12, 31).ToUniversalTime();
        
        var previousYearStart = new DateTime(today.Year - 1, 1, 1).ToUniversalTime();
        var previousYearEnd = new DateTime(today.Year - 1, 12, 31).ToUniversalTime();
        
        var trainerId = userService.GetUserId;

        var classTrainers = genericRepository.Get<ClassTrainer>(x => x.TrainerId == trainerId).ToList();
        
        var classes = genericRepository.Get<Class>(c => classTrainers.Select(ct => ct.ClassId).Contains(c.Id)).ToList();
        
        var classesCount = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Class>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.Id)),
                PreviousPeriodCount = genericRepository.GetCount<Class>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.Id))
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Class>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.Id) && x.Date >= DateOnly.FromDateTime(currentWeekStart) && x.Date <= DateOnly.FromDateTime(currentWeekStart)),
                PreviousPeriodCount = genericRepository.GetCount<Class>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.Id) && x.Date >= DateOnly.FromDateTime(previousWeekStart) && x.Date <= DateOnly.FromDateTime(previousWeekEnd))
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Class>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.Id) && x.Date >= DateOnly.FromDateTime(currentMonthStart) && x.Date <= DateOnly.FromDateTime(currentMonthEnd)),
                PreviousPeriodCount = genericRepository.GetCount<Class>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.Id) && x.Date >= DateOnly.FromDateTime(previousMonthStart) && x.Date <= DateOnly.FromDateTime(previousMonthEnd))
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Class>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.Id) && x.Date >= DateOnly.FromDateTime(currentYearStart) && x.Date <= DateOnly.FromDateTime(currentYearEnd)),
                PreviousPeriodCount = genericRepository.GetCount<Class>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.Id) && x.Date >= DateOnly.FromDateTime(previousYearStart) && x.Date <= DateOnly.FromDateTime(previousYearEnd))
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };

        var pendingAttendanceCount = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Attendance>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.ClassId) && !x.IsActionCompleted),
                PreviousPeriodCount = genericRepository.GetCount<Attendance>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.ClassId) && !x.IsActionCompleted)
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Attendance>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.ClassId) && !x.IsActionCompleted && x.CreatedAt >= currentWeekStart && x.CreatedAt <= currentWeekStart),
                PreviousPeriodCount = genericRepository.GetCount<Attendance>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.ClassId) && !x.IsActionCompleted && x.CreatedAt >= previousWeekStart && x.CreatedAt <= previousWeekEnd)
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Attendance>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.Id) && x.CreatedAt >= currentMonthStart && x.CreatedAt <= currentMonthEnd),
                PreviousPeriodCount = genericRepository.GetCount<Attendance>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.Id) && x.CreatedAt >= previousMonthStart && x.CreatedAt <= previousMonthEnd)
            },
        
            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Attendance>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.Id) && x.CreatedAt >= currentYearStart && x.CreatedAt <=currentYearEnd),
                PreviousPeriodCount = genericRepository.GetCount<Attendance>(x => classTrainers.Select(ct => ct.ClassId).Contains(x.Id) && x.CreatedAt >= previousYearStart && x.CreatedAt <= previousYearEnd)
            },
        
            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };

        var trainings = genericRepository.Get<Training>(x => classes.Select(z => z.TrainingId).Contains(x.Id)).ToList();

        var trainingInspections = genericRepository
            .Get<TrainingInspection>(x => trainings.Select(z => z.Id).Contains(x.TrainingId)).ToList();

        var questionnaires = genericRepository
            .Get<Questionnaire>(x => x.TrainingInspectionId != null && trainingInspections.Select(z => z.Id).Contains(x.TrainingInspectionId.Value)).ToList();

        var userResponses = genericRepository
            .Get<UserResponse>(x => questionnaires.Select(z => z.Id).Contains(x.QuestionId)).ToList();

        var userResponseAnalysisCount = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = genericRepository.GetCount<UserResponseAnalysis>(x => userResponses.Select(z => z.Id).Contains(x.UserResponseId) && x.CreatedBy == trainerId),
                PreviousPeriodCount = genericRepository.GetCount<UserResponseAnalysis>(x => userResponses.Select(z => z.Id).Contains(x.UserResponseId) && x.CreatedBy == trainerId)
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<UserResponseAnalysis>(x => userResponses.Select(z => z.Id).Contains(x.UserResponseId) && x.CreatedBy == trainerId && x.CreatedAt >= currentWeekStart && x.CreatedAt <= currentWeekStart),
                PreviousPeriodCount = genericRepository.GetCount<UserResponseAnalysis>(x => userResponses.Select(z => z.Id).Contains(x.UserResponseId) && x.CreatedBy == trainerId && x.CreatedAt >= previousWeekStart && x.CreatedAt <= previousWeekEnd)
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<UserResponseAnalysis>(x => userResponses.Select(z => z.Id).Contains(x.UserResponseId) && x.CreatedBy == trainerId && x.CreatedAt >= currentMonthStart && x.CreatedAt <= currentMonthEnd),
                PreviousPeriodCount = genericRepository.GetCount<UserResponseAnalysis>(x => userResponses.Select(z => z.Id).Contains(x.UserResponseId) && x.CreatedBy == trainerId && x.CreatedAt >= previousMonthStart && x.CreatedAt <= previousMonthEnd)
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<UserResponseAnalysis>(x => userResponses.Select(z => z.Id).Contains(x.UserResponseId) && x.CreatedBy == trainerId && x.CreatedAt >= currentYearStart && x.CreatedAt <= currentYearEnd),
                PreviousPeriodCount = genericRepository.GetCount<UserResponseAnalysis>(x => userResponses.Select(z => z.Id).Contains(x.UserResponseId) && x.CreatedBy == trainerId && x.CreatedAt >= previousYearStart && x.CreatedAt <= previousYearEnd)
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };
        
        var totalAssignedClassGrowth = classesCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(classesCount.CurrentPeriodCount - classesCount.PreviousPeriodCount) / classesCount.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        var pendingAttendancesGrowth = pendingAttendanceCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(pendingAttendanceCount.CurrentPeriodCount - pendingAttendanceCount.PreviousPeriodCount) / pendingAttendanceCount.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        var totalGradedInspectionsGrowth = userResponseAnalysisCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(userResponseAnalysisCount.CurrentPeriodCount - userResponseAnalysisCount.PreviousPeriodCount) / userResponseAnalysisCount.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        return new GetDashboardCount
        {
            TotalAssignedClass = classesCount.CurrentPeriodCount,
            TotalAssignedClassGrowth = totalAssignedClassGrowth,
            PendingAttendances = pendingAttendanceCount.CurrentPeriodCount,
            PendingAttendancesGrowth = pendingAttendancesGrowth,
            TotalGradedInspections = userResponseAnalysisCount.CurrentPeriodCount,
            TotalGradedInspectionsGrowth = totalGradedInspectionsGrowth,
        };
    }

    public List<GetTotalClasses> GetTotalClassesForTrainer()
    {
        var trainerId = userService.GetUserId; 
        
        var classTrainers = genericRepository.Get<ClassTrainer>(x => x.TrainerId == trainerId).ToList(); 
        
        var classIds = classTrainers.Select(ct => ct.ClassId).ToList();
        
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        var targetDates = Enumerable.Range(-1, 3)
            .Select(offset => today.AddDays(offset))
            .ToList();
        
        var classes = genericRepository.Get<Class>(c => classIds.Contains(c.Id) && targetDates.Contains(c.Date)).ToList();
        
        var classSummaries = targetDates
            .Select(date => new GetTotalClasses
            {
                Date = date.ToFormattedDate(), 
                DayOfWeek = date.DayOfWeek.ToString(),
                ClassCount = classes.Count(c => c.Date == date)
            }).ToList();
        
        return classSummaries;
    }

    public List<GetActiveTrainings> GetAllActiveTrainings()
    {
        var trainerId = userService.GetUserId;  
        
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        var classTrainers = genericRepository.Get<ClassTrainer>(x 
            => x.TrainerId == trainerId).ToList();
        
        var classes = genericRepository.Get<Class>(c => 
            !classTrainers.Select(ct => ct.ClassId).Contains(c.Id)).ToList();

        var trainings = genericRepository.Get<Training>(t => 
            classes.Select(z => z.TrainingId).Contains(t.Id) 
                && t.EndDate >= today && t.IsActive).ToList();
        
        return trainings.Select(t => new GetActiveTrainings
        {
            Id = t.Id,
            Title = t.Title,
            ImageUrl = t.ImageUrl,
            Description = t.Description,
            LocationDetails = t.LocationDetails,
            Latitude = t.Latitude ?? 0m,
            Longitude = t.Longitude ?? 0m,
            Date = $"{t.StartDate.ToFormattedDate()} - {t.EndDate.ToFormattedDate()}",
            TrainingFormatId = t.TrainingFormatId,
            TrainingFormatName = genericRepository.GetById<TrainingFormat>(t.TrainingFormatId)?.Name 
                                 ?? throw new NotFoundException("The following training format could not be found."),
            AssignedTrainer = GetAssignedTrainingsTrainers(t.Id)
        }).ToList();
    }

    public List<GetClassDetails> GetUpcomingClasses()
    {
        var userId = userService.GetUserId;
        
        var trainer = genericRepository.GetById<User>(userId)
            ?? throw new NotFoundException("The following trainer has not been registered to our system.");
        
        var classTrainers = genericRepository.Get<ClassTrainer>(x => 
            x.TrainerId == trainer.Id).ToList();
        
        var upcomingClasses = genericRepository.Get<Class>(c => 
            classTrainers.Select(ct => ct.ClassId).Contains(c.Id) 
            && c.Date.ToDateTime(TimeOnly.FromTimeSpan(c.StartTime)) >= DateTime.Now).ToList();
        
        return upcomingClasses.Select(c => new GetClassDetails
        {
            Title = c.Title,
            Location = genericRepository.GetById<Training>(c.TrainingId)?.LocationDetails ?? throw new NotFoundException("The following training could not be found."),
            ClassDate = c.Date.ToFormattedDate(),
            ClassDay = c.Date.DayOfWeek.ToString(),
            StartTime = c.StartTime.ToFormattedTime(),
            EndTime = c.EndTime.ToFormattedTime(),
            IsActive = c.IsActive,
            TrainingId = c.TrainingId,
            TrainingName = genericRepository.GetById<Training>(c.TrainingId)!.Title,
        }).ToList();
    }

    public List<GetClassesForDate> GetClassesForDatesForTrainer(DateOnly date)
    {
        var trainerId = userService.GetUserId;

        var classTrainers = genericRepository.Get<ClassTrainer>(x => x.TrainerId == trainerId).ToList();

        var classes = genericRepository.Get<Class>(c => 
            classTrainers.Select(ct => ct.ClassId).Contains(c.Id) 
                && c.Date == date && c.IsActive).ToList();

        return classes.Select(c => new GetClassesForDate
        {
            Title = c.Title,
            ClassDate = c.Date.ToFormattedDate(),
            ClassDay = c.Date.DayOfWeek.ToString(),
            ClassImage = c.ImageUrl ?? "",
            StartTime = c.StartTime.ToFormattedTime(),
            EndTime = c.EndTime.ToFormattedTime(),
            IsActive = c.IsActive,
            TrainingId = c.TrainingId,
            ClassId = c.Id,
            TrainingName = genericRepository.GetById<Training>(c.TrainingId)?.Title ?? throw new NotFoundException("The following training could not be found."), 
        }).ToList();
    }

    public List<GetClassDetails> GetCompletedClasses()
    {
        var trainerId = userService.GetUserId;
        
        var today = DateOnly.FromDateTime(DateTime.Today);

        var currentTime = DateTime.Now.TimeOfDay;

        var classTrainers = genericRepository.Get<ClassTrainer>(x => 
            x.TrainerId == trainerId).ToList();

        var classes = genericRepository.Get<Class>(c => 
            classTrainers.Select(ct => ct.ClassId).Contains(c.Id) 
                && c.Date <= today && c.StartTime < currentTime && c.EndTime < currentTime).ToList();
        
        return classes.Select(c => new GetClassDetails
        {
            Title = c.Title,
            Location = genericRepository.GetById<Training>(c.TrainingId)?.LocationDetails ?? throw new NotFoundException("The following training could not be found."),
            ClassDate = c.Date.ToFormattedDate(),
            ClassDay = c.Date.DayOfWeek.ToString(),
            StartTime = c.StartTime.ToFormattedTime(),
            EndTime = c.EndTime.ToFormattedTime(),
            IsActive = c.IsActive,
            TrainingId = c.TrainingId,
            TrainingName = genericRepository.GetById<Training>(c.TrainingId)!.Title,
        }).ToList();
    }
    #endregion
    
    #region Candidates 
    public GetCandidateTrainingProgressDto GetTrainingProgressesForCandidate(int period)
    {
        var today = DateTime.UtcNow;
        
        var currentWeekStart = today.AddDays(-(int)today.DayOfWeek);  
        var currentWeekEnd = currentWeekStart.AddDays(6);       
        
        var previousWeekStart = currentWeekStart.AddDays(-7);         
        var previousWeekEnd = currentWeekStart.AddDays(-1);  
        
        var currentMonthStart = new DateTime(today.Year, today.Month, 1).ToUniversalTime();
        var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1); 
        
        var previousMonthStart = currentMonthStart.AddMonths(-1).ToUniversalTime(); 
        var previousMonthEnd = currentMonthStart.AddDays(-1);   
        
        var currentYearStart = new DateTime(today.Year, 1, 1).ToUniversalTime();
        var currentYearEnd = new DateTime(today.Year, 12, 31).ToUniversalTime();
        
        var previousYearStart = new DateTime(today.Year - 1, 1, 1).ToUniversalTime();
        var previousYearEnd = new DateTime(today.Year - 1, 12, 31).ToUniversalTime();
        
        var candidateId = userService.GetUserId;

        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x => 
            x.CandidateId == candidateId && x.IsApproved && x.IsActionCompleted).ToList();

        var trainings = genericRepository.Get<Training>(x => 
            trainingCandidates.Select(z => z.TrainingId).Contains(x.Id)).ToList();
        
        var trainingsCompletedCount = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate)
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate && x.StartDate >= DateOnly.FromDateTime(currentWeekStart) && x.StartDate <= DateOnly.FromDateTime(currentWeekStart)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate && x.StartDate >= DateOnly.FromDateTime(previousWeekStart) && x.StartDate <= DateOnly.FromDateTime(previousWeekEnd))
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate && x.StartDate >= DateOnly.FromDateTime(currentMonthStart) && x.StartDate <= DateOnly.FromDateTime(currentMonthEnd)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate && x.StartDate >= DateOnly.FromDateTime(previousMonthStart) && x.StartDate <= DateOnly.FromDateTime(previousMonthEnd))
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate && x.StartDate >= DateOnly.FromDateTime(currentYearStart) && x.StartDate <= DateOnly.FromDateTime(currentYearEnd)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate && x.StartDate >= DateOnly.FromDateTime(previousYearStart) && x.StartDate <= DateOnly.FromDateTime(previousYearEnd))
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };
        
        var trainingsInProgress = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now))
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now) && x.StartDate >= DateOnly.FromDateTime(currentWeekStart) && x.StartDate <= DateOnly.FromDateTime(currentWeekStart)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now) && x.StartDate >= DateOnly.FromDateTime(previousWeekStart) && x.StartDate <= DateOnly.FromDateTime(previousWeekEnd))
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now) && x.StartDate >= DateOnly.FromDateTime(currentMonthStart) && x.StartDate <= DateOnly.FromDateTime(currentMonthEnd)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now) && x.StartDate >= DateOnly.FromDateTime(previousMonthStart) && x.StartDate <= DateOnly.FromDateTime(previousMonthEnd))
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now) && x.StartDate >= DateOnly.FromDateTime(currentYearStart) && x.StartDate <= DateOnly.FromDateTime(currentYearEnd)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now) && x.StartDate >= DateOnly.FromDateTime(previousYearStart) && x.StartDate <= DateOnly.FromDateTime(previousYearEnd))
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };

        var classes = genericRepository.Get<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId)).ToList();
        
        var classesCount = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId)),
                PreviousPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId))
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId) && x.Date >= DateOnly.FromDateTime(currentWeekStart) && x.Date <= DateOnly.FromDateTime(currentWeekStart)),
                PreviousPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId) && x.Date >= DateOnly.FromDateTime(previousWeekStart) && x.Date <= DateOnly.FromDateTime(previousWeekEnd))
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId) && x.Date >= DateOnly.FromDateTime(currentMonthStart) && x.Date <= DateOnly.FromDateTime(currentMonthEnd)),
                PreviousPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId) && x.Date >= DateOnly.FromDateTime(previousMonthStart) && x.Date <= DateOnly.FromDateTime(previousMonthEnd))
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId) && x.Date >= DateOnly.FromDateTime(currentYearStart) && x.Date <= DateOnly.FromDateTime(currentYearEnd)),
                PreviousPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId) && x.Date >= DateOnly.FromDateTime(previousYearStart) && x.Date <= DateOnly.FromDateTime(previousYearEnd))
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };

        var attendances = genericRepository.Get<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && x.CandidateId == candidateId).ToList();
        
        var attendancesCount = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && x.CandidateId == candidateId),
                PreviousPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && x.CandidateId == candidateId)
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && x.CandidateId == candidateId && x.CreatedAt >= currentWeekStart && x.CreatedAt <= currentWeekStart),
                PreviousPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && x.CandidateId == candidateId && x.CreatedAt >= previousWeekStart && x.CreatedAt <= previousWeekEnd)
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && x.CandidateId == candidateId && x.CreatedAt >= currentMonthStart && x.CreatedAt <= currentMonthEnd),
                PreviousPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && x.CandidateId == candidateId && x.CreatedAt >= previousMonthStart && x.CreatedAt <= previousMonthEnd)
            },
        
            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && x.CandidateId == candidateId && x.CreatedAt >= currentYearStart && x.CreatedAt <= currentYearEnd),
                PreviousPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && x.CandidateId == candidateId && x.CreatedAt >= previousYearStart && x.CreatedAt <= previousYearEnd)
            },
        
            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };

        var certificates = genericRepository.Get<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId));
        
         var certificatesCount = period switch
         {
             Constants.TimePeriod.All => new
             {
                 CurrentPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId)),
                 PreviousPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId))
             },
             Constants.TimePeriod.Weekly => new
             {
                 CurrentPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId) && x.CreatedAt >= currentWeekStart && x.CreatedAt <= currentWeekStart),
                 PreviousPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId) && x.CreatedAt >= previousWeekStart && x.CreatedAt <= previousWeekEnd)
             },
             Constants.TimePeriod.Monthly => new
             {
                 CurrentPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId) && x.CreatedAt >= currentMonthStart && x.CreatedAt <= currentMonthEnd),
                 PreviousPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId) && x.CreatedAt >= previousMonthStart && x.CreatedAt <= previousMonthEnd)
             },
        
             Constants.TimePeriod.Yearly => new
             {
                 CurrentPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId) && x.CreatedAt >= currentYearStart && x.CreatedAt <= currentYearEnd),
                 PreviousPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId) && x.CreatedAt >= previousYearStart && x.CreatedAt <= previousYearEnd)
             },
        
             _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
         };
        
        double attendanceClassesGrowth = attendancesCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(attendancesCount.CurrentPeriodCount - attendancesCount.PreviousPeriodCount) / attendancesCount.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        double totalPossibleAttendancesGrowth = classesCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(classesCount.CurrentPeriodCount - classesCount.PreviousPeriodCount) / classesCount.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        double trainingsCompletedGrowth = trainingsCompletedCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(trainingsCompletedCount.CurrentPeriodCount - trainingsCompletedCount.PreviousPeriodCount) / trainingsCompletedCount.PreviousPeriodCount) * 100, 0)
            : 0;
        
        double trainingsInProgressGrowth = trainingsInProgress.PreviousPeriodCount > 0 
            ? Math.Round(((double)(trainingsInProgress.CurrentPeriodCount - trainingsInProgress.PreviousPeriodCount) / trainingsInProgress.PreviousPeriodCount) * 100, 0)
            : 0;
        
        double certificationsEarnedGrowth = certificatesCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(certificatesCount.CurrentPeriodCount - certificatesCount.PreviousPeriodCount) / certificatesCount.PreviousPeriodCount) * 100, 0)
            : 0;

        return new GetCandidateTrainingProgressDto
        {
            AttendedClasses = attendancesCount.CurrentPeriodCount,
            AttendedClassesGrowth = attendanceClassesGrowth,
            TotalPossibleAttendances = classesCount.CurrentPeriodCount,
            TotalPossibleAttendancesGrowth = totalPossibleAttendancesGrowth,
            TrainingsCompleted = trainingsCompletedCount.CurrentPeriodCount,
            TrainingsCompletedGrowth = trainingsCompletedGrowth,
            TrainingsInProgress = trainingsInProgress.CurrentPeriodCount,
            TrainingsInProgressGrowth = trainingsInProgressGrowth,
            CertificationsEarned = certificatesCount.CurrentPeriodCount,
            CertificationsEarnedGrowth = certificationsEarnedGrowth
        };
    }

    public List<GetAssignedTrainingDto> GetAssignedTrainingsForCandidate()
    {
        var candidateId = userService.GetUserId;

        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x 
            => x.CandidateId == candidateId && x.IsApproved).ToList();

        var trainings = genericRepository.Get<Training>(x => 
            trainingCandidates.Select(z => z.TrainingId).Contains(x.Id)).ToList();

        var classes = genericRepository.Get<Class>(x => 
            trainings.Select(z => z.Id).Contains(x.TrainingId) && x.StartTime > DateTime.Now.TimeOfDay).ToList();

        var classTrainers = genericRepository.Get<ClassTrainer>(x => 
                classes.Select(z => z.Id).Contains(x.ClassId)).ToList();
        
        var trainers = genericRepository.Get<User>(x => 
            classTrainers.Select(z => z.TrainerId).Contains(x.Id)).ToList().DistinctBy(z => z.Id).ToList();
        
        var result = new List<GetAssignedTrainingDto>();
        
        foreach (var training in trainings)
        {
            var nextClass = classes.Count > 0 
                ? classes.Where(x => x.TrainingId == training.Id).OrderBy(x => x.Date).ThenBy(x => x.StartTime).FirstOrDefault() 
                : null;

            var trainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)?.Name ??
                                 throw new NotFoundException("The following training format could not be found.");
            
            var assignedTrainings = new GetAssignedTrainingDto
            {
                Id = training.Id,
                Name = $"{training.Title} ({trainingFormat})",
                Description = training.Description,
                ImageUrl = training.ImageUrl,
                NextClassDate = nextClass?.Date.ToFormattedDate() ?? "No classes assigned yet.",
                NextClassTime = nextClass != null 
                    ? $"{nextClass.StartTime.ToFormattedTime()} - {nextClass.EndTime.ToFormattedTime()}"
                    : string.Empty,
                Trainers = trainers.Select(x => new AssignedTrainersDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImageUrl = x.ImageURL
                }).ToList()
            };

            result.Add(assignedTrainings);
        }

        return result;
    }

    public List<GetNewTrainingsDto> GetNewTrainingsForCandidate()
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
            ?? throw new NotFoundException("The candidate is not registered in our system.");

        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x => 
            x.CandidateId == candidate.Id).ToList();

        var unassignedTrainings = genericRepository.Get<Training>(x => 
            !trainingCandidates.Select(z => z.TrainingId).Contains(x.Id) && x.StartDate >= DateOnly.FromDateTime(DateTime.Now)).ToList();

        var classes = genericRepository.Get<Class>(x => 
            unassignedTrainings.Select(z => z.Id).Contains(x.TrainingId) && x.StartTime > DateTime.Now.TimeOfDay).ToList();

        var result = new List<GetNewTrainingsDto>();

        foreach (var training in unassignedTrainings)
        {
            var nextClass = classes.Count > 0 
                ? classes.OrderBy(x => x.Date).ThenBy(x => x.StartTime).FirstOrDefault() 
                : null;

            var classTrainers = genericRepository.Get<ClassTrainer>(x => 
                classes.Select(z => z.TrainingId).Contains(training.Id)).ToList();
        
            var trainers = genericRepository.Get<User>(x => 
                classTrainers.Select(z => z.TrainerId).Contains(x.Id)).ToList().DistinctBy(z => z.Id).ToList();

            var newTrainings = new GetNewTrainingsDto
            {
                Id = training.Id,
                Name = $"{training.Title}",
                ImageUrl = training.ImageUrl,
                Description = training.Description,
                NextClassDate = nextClass?.Date.ToFormattedDate() ?? "No classes assigned yet.",
                NextClassTime = nextClass != null 
                    ? $"{nextClass.StartTime.ToFormattedTime()} - {nextClass.EndTime.ToFormattedTime()}"
                    : string.Empty,
                Trainers = trainers.Select(x => new AssignedTrainersDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImageUrl = x.ImageURL
                }).ToList()
            };

            result.Add(newTrainings);
        }

        return result;
    }
    
    public List<GetClassesForDate> GetClassesForDatesForCandidates(DateOnly date)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId) ??
                        throw new NotFoundException("The following user has not been registered to our system.");

        var trainingCandidates =
            genericRepository.Get<TrainingCandidate>(x => x.CandidateId == candidate.Id && x.IsApproved).ToList();
        
        var classes = genericRepository.Get<Class>(c => 
            trainingCandidates.Select(ct => ct.TrainingId).Contains(c.TrainingId) 
                && c.Date == date && c.IsActive).ToList();

        return classes.Select(c => new GetClassesForDate
        {
            Title = c.Title,
            ClassDate = c.Date.ToFormattedDate(),
            ClassDay = c.Date.DayOfWeek.ToString(),
            ClassImage = c.ImageUrl ?? "",
            StartTime = c.StartTime.ToFormattedTime(),
            EndTime = c.EndTime.ToFormattedTime(),
            IsActive = c.IsActive,
            TrainingId = c.TrainingId,
            ClassId = c.Id,
            TrainingName = genericRepository.GetById<Training>(c.TrainingId)?.Title ?? throw new NotFoundException("The following training could not be found."), 
        }).ToList();
    }

    public List<GetQuestionnaireDto> GetUnansweredQuestionnaireDetailsForCandidate()
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId) ??
                        throw new NotFoundException("The following user has not been registered to our system.");

        var trainingCandidates =
            genericRepository.Get<TrainingCandidate>(x => x.CandidateId == candidate.Id && x.IsApproved).ToList();

        var trainingIds = trainingCandidates.Select(z => z.TrainingId).ToList();

        var result = new List<GetQuestionnaireDto>();
        
        foreach (var trainingId in trainingIds)
        {
            var training = genericRepository.GetById<Training>(trainingId) ??
                           throw new NotFoundException("The following training could not be found.");

            var trainingInspections = genericRepository.Get<TrainingInspection>(x => 
                x.TrainingId == training.Id).ToList();

            var questionnaires = genericRepository.Get<Questionnaire>(x => 
                    x.TrainingInspectionId != null && trainingInspections.Select(z => z.Id).Contains(x.TrainingInspectionId.Value)).ToList();

            foreach (var questionnaire in questionnaires)
            {
                var trainingInspection = genericRepository.GetFirstOrDefault<TrainingInspection>(x => 
                    x.Id == questionnaire.TrainingInspectionId) ?? throw new NotFoundException("The following training inspection could not be found.");

                var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId) ??
                                 throw new NotFoundException("The following inspection could not be found.");
                
                var userResponse = genericRepository.GetFirstOrDefault<UserResponse>(x => 
                    questionnaire.Id == x.QuestionId && x.IsAnsweredByCandidate && x.CandidateId == candidate.Id);
                
                if (userResponse is not null) continue;
                
                result.Add(new GetQuestionnaireDto()
                {
                    Id = questionnaire.Id,
                    TrainingId = training.Id,
                    Training = training.Title,
                    Inspection = inspection.Name,
                    InspectionId = inspection.Id,
                    UploadedDate = questionnaire.CreatedAt.ToFormattedDateTime()
                });
            }
        }

        return result;
    }
    #endregion
    
    #region Clients
    public GetClientTrainingProgressDto GetTrainingProgressesForClient(int period)
    {
        var today = DateTime.UtcNow;
        
        var currentWeekStart = today.AddDays(-(int)today.DayOfWeek);  
        var currentWeekEnd = currentWeekStart.AddDays(6);       
        
        var previousWeekStart = currentWeekStart.AddDays(-7);         
        var previousWeekEnd = currentWeekStart.AddDays(-1);  
        
        var currentMonthStart = new DateTime(today.Year, today.Month, 1).ToUniversalTime();
        var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1); 
        
        var previousMonthStart = currentMonthStart.AddMonths(-1).ToUniversalTime(); 
        var previousMonthEnd = currentMonthStart.AddDays(-1);   
        
        var currentYearStart = new DateTime(today.Year, 1, 1).ToUniversalTime();
        var currentYearEnd = new DateTime(today.Year, 12, 31).ToUniversalTime();
        
        var previousYearStart = new DateTime(today.Year - 1, 1, 1).ToUniversalTime();
        var previousYearEnd = new DateTime(today.Year - 1, 12, 31).ToUniversalTime();
        
        var clientUserId = userService.GetUserId;

        var clientUser = genericRepository.GetById<User>(clientUserId) ??
                         throw new NotFoundException("The following client has not been registered to our system.");

        if (clientUser.OrganizationId is null)
            throw new NotFoundException("The following user has not been registered as a client administrator.");
        
        var candidates = genericRepository.Get<User>(x => x.OrganizationId == clientUser.OrganizationId).ToList();
        
        var candidatesCount = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = genericRepository.GetCount<User>(x => x.OrganizationId == clientUser.OrganizationId),
                PreviousPeriodCount = genericRepository.GetCount<User>(x => x.OrganizationId == clientUser.OrganizationId)
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<User>(x => x.OrganizationId == clientUser.OrganizationId && x.RegisteredDate >= currentWeekStart && x.RegisteredDate <= currentWeekStart),
                PreviousPeriodCount = genericRepository.GetCount<User>(x => x.OrganizationId == clientUser.OrganizationId && x.RegisteredDate >= previousWeekStart && x.RegisteredDate <= previousWeekEnd)
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<User>(x => x.OrganizationId == clientUser.OrganizationId && x.RegisteredDate >= currentMonthStart && x.RegisteredDate <= currentMonthEnd),
                PreviousPeriodCount = genericRepository.GetCount<User>(x => x.OrganizationId == clientUser.OrganizationId && x.RegisteredDate >= previousMonthStart && x.RegisteredDate <= previousMonthEnd)
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<User>(x => x.OrganizationId == clientUser.OrganizationId && x.RegisteredDate >= currentYearStart && x.RegisteredDate <= currentYearEnd),
                PreviousPeriodCount = genericRepository.GetCount<User>(x => x.OrganizationId == clientUser.OrganizationId && x.RegisteredDate >= previousYearStart && x.RegisteredDate <= previousYearEnd)
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };

        var candidateIds = candidates.Select(x => x.Id);
        
        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x => 
            candidateIds.Contains(x.CandidateId) && x.IsApproved && x.IsActionCompleted).ToList();

        var trainings = genericRepository.Get<Training>(x => 
            trainingCandidates.Select(z => z.TrainingId).Contains(x.Id)).ToList();
        
        var trainingsCompletedCount = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate)
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate && x.StartDate >= DateOnly.FromDateTime(currentWeekStart) && x.StartDate <= DateOnly.FromDateTime(currentWeekStart)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate && x.StartDate >= DateOnly.FromDateTime(previousWeekStart) && x.StartDate <= DateOnly.FromDateTime(previousWeekEnd))
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate && x.StartDate >= DateOnly.FromDateTime(currentMonthStart) && x.StartDate <= DateOnly.FromDateTime(currentMonthEnd)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate && x.StartDate >= DateOnly.FromDateTime(previousMonthStart) && x.StartDate <= DateOnly.FromDateTime(previousMonthEnd))
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate && x.StartDate >= DateOnly.FromDateTime(currentYearStart) && x.StartDate <= DateOnly.FromDateTime(currentYearEnd)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) > x.EndDate && x.StartDate >= DateOnly.FromDateTime(previousYearStart) && x.StartDate <= DateOnly.FromDateTime(previousYearEnd))
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };
        
        var trainingsInProgressCount = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now))
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now) && x.StartDate >= DateOnly.FromDateTime(currentWeekStart) && x.StartDate <= DateOnly.FromDateTime(currentWeekStart)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now) && x.StartDate >= DateOnly.FromDateTime(previousWeekStart) && x.StartDate <= DateOnly.FromDateTime(previousWeekEnd))
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now) && x.StartDate >= DateOnly.FromDateTime(currentMonthStart) && x.StartDate <= DateOnly.FromDateTime(currentMonthEnd)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now) && x.StartDate >= DateOnly.FromDateTime(previousMonthStart) && x.StartDate <= DateOnly.FromDateTime(previousMonthEnd))
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now) && x.StartDate >= DateOnly.FromDateTime(currentYearStart) && x.StartDate <= DateOnly.FromDateTime(currentYearEnd)),
                PreviousPeriodCount = trainings.Count(x => DateOnly.FromDateTime(DateTime.Now) < x.EndDate && x.StartDate > DateOnly.FromDateTime(DateTime.Now) && x.StartDate >= DateOnly.FromDateTime(previousYearStart) && x.StartDate <= DateOnly.FromDateTime(previousYearEnd))
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };

        var classes = genericRepository.Get<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId)).ToList();
        
        var classesCount = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId)),
                PreviousPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId))
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId) && x.Date >= DateOnly.FromDateTime(currentWeekStart) && x.Date <= DateOnly.FromDateTime(currentWeekStart)),
                PreviousPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId) && x.Date >= DateOnly.FromDateTime(previousWeekStart) && x.Date <= DateOnly.FromDateTime(previousWeekEnd))
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId) && x.Date >= DateOnly.FromDateTime(currentMonthStart) && x.Date <= DateOnly.FromDateTime(currentMonthEnd)),
                PreviousPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId) && x.Date >= DateOnly.FromDateTime(previousMonthStart) && x.Date <= DateOnly.FromDateTime(previousMonthEnd))
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId) && x.Date >= DateOnly.FromDateTime(currentYearStart) && x.Date <= DateOnly.FromDateTime(currentYearEnd)),
                PreviousPeriodCount = genericRepository.GetCount<Class>(x => trainings.Select(z => z.Id).Contains(x.TrainingId) && x.Date >= DateOnly.FromDateTime(previousYearStart) && x.Date <= DateOnly.FromDateTime(previousYearEnd))
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };

        var attendances = genericRepository.Get<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && candidateIds.Contains(x.CandidateId)).ToList();
        
        var attendancesCount = period switch
        {
            Constants.TimePeriod.All => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && candidateIds.Contains(x.CandidateId)),
                PreviousPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && candidateIds.Contains(x.CandidateId))
            },
            Constants.TimePeriod.Weekly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && candidateIds.Contains(x.CandidateId) && x.CreatedAt >= currentWeekStart && x.CreatedAt <= currentWeekStart),
                PreviousPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && candidateIds.Contains(x.CandidateId) && x.CreatedAt >= previousWeekStart && x.CreatedAt <= previousWeekEnd)
            },
            Constants.TimePeriod.Monthly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && candidateIds.Contains(x.CandidateId) && x.CreatedAt >= currentMonthStart && x.CreatedAt <= currentMonthEnd),
                PreviousPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && candidateIds.Contains(x.CandidateId) && x.CreatedAt >= previousMonthStart && x.CreatedAt <= previousMonthEnd)
            },

            Constants.TimePeriod.Yearly => new
            {
                CurrentPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && candidateIds.Contains(x.CandidateId) && x.CreatedAt >= currentYearStart && x.CreatedAt <= currentYearEnd),
                PreviousPeriodCount = genericRepository.GetCount<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.IsApproved && candidateIds.Contains(x.CandidateId) && x.CreatedAt >= previousYearStart && x.CreatedAt <= previousYearEnd)
            },

            _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
        };

        var certificates = genericRepository.Get<Certificate>(x => 
            trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId));
        
         var certificatesCount = period switch
         {
             Constants.TimePeriod.All => new
             {
                 CurrentPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId)),
                 PreviousPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId))
             },
             Constants.TimePeriod.Weekly => new
             {
                 CurrentPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId) && x.CreatedAt >= currentWeekStart && x.CreatedAt <= currentWeekStart),
                 PreviousPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId) && x.CreatedAt >= previousWeekStart && x.CreatedAt <= previousWeekEnd)
             },
             Constants.TimePeriod.Monthly => new
             {
                 CurrentPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId) && x.CreatedAt >= currentMonthStart && x.CreatedAt <= currentMonthEnd),
                 PreviousPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId) && x.CreatedAt >= previousMonthStart && x.CreatedAt <= previousMonthEnd)
             },
        
             Constants.TimePeriod.Yearly => new
             {
                 CurrentPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId) && x.CreatedAt >= currentYearStart && x.CreatedAt <= currentYearEnd),
                 PreviousPeriodCount = genericRepository.GetCount<Certificate>(x => trainingCandidates.Select(z => z.Id).Contains(x.TrainingCandidateId) && x.CreatedAt >= previousYearStart && x.CreatedAt <= previousYearEnd)
             },
        
             _ => new { CurrentPeriodCount = 0, PreviousPeriodCount = 0 }
         };
        
        double totalRegisteredCandidatesGrowth = candidatesCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(candidatesCount.CurrentPeriodCount - candidatesCount.PreviousPeriodCount) / candidatesCount.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        double attendedClassesGrowth = attendancesCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(attendancesCount.CurrentPeriodCount - attendancesCount.PreviousPeriodCount) / attendancesCount.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        double totalPossibleAttendancesGrowth = classesCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(classesCount.CurrentPeriodCount - classesCount.PreviousPeriodCount) / classesCount.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        double trainingsCompletedGrowth = trainingsCompletedCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(trainingsCompletedCount.CurrentPeriodCount - trainingsCompletedCount.PreviousPeriodCount) / trainingsCompletedCount.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        double trainingsInProgressGrowth = trainingsInProgressCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(trainingsInProgressCount.CurrentPeriodCount - trainingsInProgressCount.PreviousPeriodCount) / trainingsInProgressCount.PreviousPeriodCount) * 100, 0)
            : 0; 
        
        double certificationsEarnedGrowth = certificatesCount.PreviousPeriodCount > 0 
            ? Math.Round(((double)(certificatesCount.CurrentPeriodCount - certificatesCount.PreviousPeriodCount) / certificatesCount.PreviousPeriodCount) * 100, 0)
            : 0; 

        return new GetClientTrainingProgressDto
        {
            TotalRegisteredCandidates = candidatesCount.CurrentPeriodCount - 1,
            TotalRegisteredCandidatesGrowth = totalRegisteredCandidatesGrowth,
            AttendedClasses = attendancesCount.CurrentPeriodCount,
            AttendedClassesGrowth = attendedClassesGrowth,
            TotalPossibleAttendances = classesCount.CurrentPeriodCount,
            TotalPossibleAttendancesGrowth = totalPossibleAttendancesGrowth,
            TrainingsCompleted = trainingsCompletedCount.CurrentPeriodCount,
            TrainingsCompletedGrowth = trainingsCompletedGrowth,
            TrainingsInProgress = trainingsInProgressCount.CurrentPeriodCount,
            TrainingsInProgressGrowth = trainingsInProgressGrowth,
            CertificationsEarned = certificatesCount.CurrentPeriodCount,
            CertificationsEarnedGrowth = certificationsEarnedGrowth
        };
    }

    public List<GetAssignedTrainingDto> GetAssignedTrainingsForClient()
    {
        var clientUserId = userService.GetUserId;

        var clientUser = genericRepository.GetById<User>(clientUserId) ??
                         throw new NotFoundException("The following client has not been registered to our system.");

        if (clientUser.OrganizationId is null)
            throw new NotFoundException("The following user has not been registered as a client administrator.");
        
        var candidates = genericRepository.Get<User>(x => x.OrganizationId == clientUser.OrganizationId).ToList();

        var candidateIds = candidates.Select(x => x.Id);
        
        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x 
            => candidateIds.Contains(x.CandidateId) && x.IsApproved).ToList();

        var trainings = genericRepository.Get<Training>(x => 
            trainingCandidates.Select(z => z.TrainingId).Contains(x.Id)).ToList();

        var classes = genericRepository.Get<Class>(x => 
            trainings.Select(z => z.Id).Contains(x.TrainingId) && x.StartTime > DateTime.Now.TimeOfDay).ToList();

        var classTrainers = genericRepository.Get<ClassTrainer>(x => 
            classes.Select(z => z.Id).Contains(x.ClassId)).ToList();
        
        var trainers = genericRepository.Get<User>(x => 
            classTrainers.Select(z => z.TrainerId).Contains(x.Id)).ToList().DistinctBy(z => z.Id).ToList();
        
        var result = new List<GetAssignedTrainingDto>();

        foreach (var training in trainings)
        {
            var nextClass = classes.Count > 0 
                ? classes.OrderBy(x => x.Date).ThenBy(x => x.StartTime).FirstOrDefault() 
                : null;

            var trainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)?.Name ??
                                 throw new NotFoundException("The following training format could not be found.");

            var trainingCandidatesForTraining =
                trainingCandidates.Where(x => x.TrainingId == training.Id && x.IsApproved).ToList();

            var candidatesForTraining = candidates
                .Where(x => trainingCandidatesForTraining.Select(z => z.CandidateId).Contains(x.Id)).ToList();
            
            var assignedTrainings = new GetAssignedTrainingDto
            {
                Id = training.Id, 
                Name = $"{training.Title} ({trainingFormat})",
                ImageUrl = training.ImageUrl,
                Description = training.Description,
                NextClassDate = nextClass?.Date.ToFormattedDate() ?? "No classes assigned yet.",
                NextClassTime = nextClass != null 
                    ? $"{nextClass.StartTime.ToFormattedTime()} - {nextClass.EndTime.ToFormattedTime()}"
                    : string.Empty,
                Trainers = trainers.Select(x => new AssignedTrainersDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImageUrl = x.ImageURL
                }).ToList(),
                Candidates = candidatesForTraining.Select(x => new AssignedCandidatesDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImageUrl = x.ImageURL
                }).ToList()
            };

            result.Add(assignedTrainings);
        }

        return result;
    }

    public List<GetNewTrainingsDto> GetNewTrainingsForClient()
    {
        var clientUserId = userService.GetUserId;

        var clientUser = genericRepository.GetById<User>(clientUserId) ??
                         throw new NotFoundException("The following client has not been registered to our system.");

        if (clientUser.OrganizationId is null)
            throw new NotFoundException("The following user has not been registered as a client administrator.");
        
        var candidates = genericRepository.Get<User>(x => x.OrganizationId == clientUser.OrganizationId).ToList();

        var candidateIds = candidates.Select(x => x.Id);

        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x => 
            candidateIds.Contains(x.CandidateId)).ToList();

        var unassignedTrainings = genericRepository.Get<Training>(x => 
            !trainingCandidates.Select(z => z.TrainingId).Contains(x.Id) && x.StartDate > DateOnly.FromDateTime(DateTime.UtcNow)).ToList();

        var classes = genericRepository.Get<Class>(x => 
            unassignedTrainings.Select(z => z.Id).Contains(x.TrainingId) && x.StartTime > DateTime.Now.TimeOfDay).ToList();

        var result = new List<GetNewTrainingsDto>();

        foreach (var training in unassignedTrainings)
        {
            var nextClass = classes.Count > 0 
                ? classes.OrderBy(x => x.Date).ThenBy(x => x.StartTime).FirstOrDefault() 
                : null;

            var classTrainers = genericRepository.Get<ClassTrainer>(x => 
                classes.Select(z => z.TrainingId).Contains(training.Id)).ToList();
        
            var trainers = genericRepository.Get<User>(x => 
                classTrainers.Select(z => z.TrainerId).Contains(x.Id)).ToList().DistinctBy(z => z.Id).ToList();
            
            var newTrainings = new GetNewTrainingsDto
            {
                Id = training.Id,
                Name = $"{training.Title}",
                ImageUrl = training.ImageUrl,
                Description = training.Description,
                NextClassDate = nextClass?.Date.ToFormattedDate() ?? "No classes assigned yet.",
                NextClassTime = nextClass != null 
                    ? $"{nextClass.StartTime.ToFormattedTime()} - {nextClass.EndTime.ToFormattedTime()}"
                    : string.Empty,
                Trainers = trainers.Select(x => new AssignedTrainersDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImageUrl = x.ImageURL
                }).ToList()
            };

            result.Add(newTrainings);
        }

        return result;
    }
    
    public List<GetClassesForDate> GetClassesForDatesForClient(DateOnly date)
    {
        var clientUserId = userService.GetUserId;

        var clientUser = genericRepository.GetById<User>(clientUserId) ??
                         throw new NotFoundException("The following client has not been registered to our system.");

        if (clientUser.OrganizationId is null)
            throw new NotFoundException("The following user has not been registered as a client administrator.");
        
        var candidates = genericRepository.Get<User>(x => x.OrganizationId == clientUser.OrganizationId).ToList();

        var candidateIds = candidates.Select(x => x.Id).ToList();

        var trainingCandidates =
            genericRepository.Get<TrainingCandidate>(x => candidateIds.Contains(x.CandidateId) && x.IsApproved).ToList();
        
        var classes = genericRepository.Get<Class>(c => 
            trainingCandidates.Select(ct => ct.TrainingId).Contains(c.TrainingId) 
                && c.Date == date && c.IsActive).ToList();

        return classes.Select(c => new GetClassesForDate
        {
            Title = c.Title,
            ClassDate = c.Date.ToFormattedDate(),
            ClassDay = c.Date.DayOfWeek.ToString(),
            ClassImage = c.ImageUrl ?? "",
            StartTime = c.StartTime.ToFormattedTime(),
            EndTime = c.EndTime.ToFormattedTime(),
            IsActive = c.IsActive,
            TrainingId = c.TrainingId,
            ClassId = c.Id,
            TrainingName = genericRepository.GetById<Training>(c.TrainingId)?.Title ?? throw new NotFoundException("The following training could not be found."), 
        }).ToList();
    }

    public List<GetQuestionnaireDto> GetUnansweredQuestionnaireDetailsForClient()
    {
        var clientUserId = userService.GetUserId;

        var clientUser = genericRepository.GetById<User>(clientUserId) ??
                         throw new NotFoundException("The following client has not been registered to our system.");

        if (clientUser.OrganizationId is null)
            throw new NotFoundException("The following user has not been registered as a client administrator.");
        
        var candidates = genericRepository.Get<User>(x => x.OrganizationId == clientUser.OrganizationId).ToList();

        var candidateIds = candidates.Select(x => x.Id);

        var trainingCandidates =
            genericRepository.Get<TrainingCandidate>(x => candidateIds.Contains(x.CandidateId) && x.IsApproved).ToList();

        var trainingIds = trainingCandidates.Select(z => z.TrainingId).ToList();

        var result = new List<GetQuestionnaireDto>();
        
        foreach (var trainingId in trainingIds)
        {
            var training = genericRepository.GetById<Training>(trainingId) ??
                           throw new NotFoundException("The following training could not be found.");

            var trainingInspections = genericRepository.Get<TrainingInspection>(x => 
                x.TrainingId == training.Id).ToList();

            var questionnaires = genericRepository.Get<Questionnaire>(x => 
                    x.TrainingInspectionId != null && trainingInspections.Select(z => z.Id).Contains(x.TrainingInspectionId.Value)).ToList();

            foreach (var questionnaire in questionnaires)
            {
                var trainingInspection = genericRepository.GetFirstOrDefault<TrainingInspection>(x => 
                    x.Id == questionnaire.TrainingInspectionId) ?? throw new NotFoundException("The following training inspection could not be found.");

                var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId) ??
                                 throw new NotFoundException("The following inspection could not be found.");
                
                var userResponse = genericRepository.GetFirstOrDefault<UserResponse>(x => 
                    questionnaire.Id == x.QuestionId && x.IsAnsweredByCandidate && candidateIds.Contains(x.CandidateId));
                
                if (userResponse is not null) continue;
                
                result.Add(new GetQuestionnaireDto()
                {
                    Id = questionnaire.Id,
                    TrainingId = training.Id,
                    Training = training.Title,
                    Inspection = inspection.Name,
                    InspectionId = inspection.Id,
                    UploadedDate = questionnaire.CreatedAt.ToFormattedDateTime()
                });
            }
        }

        return result;
    }

    public List<GetAllClassesDto> GetAllClassesForUser()
    {
        var userId = userService.GetUserId;
        
        var user = genericRepository.GetById<User>(userId)
            ?? throw new NotFoundException("The following user has not been registered to our system.");
        
        var role = userService.GetUserRole;
        
        switch (role)
        {
            case Constants.Roles.Candidate:
            {
                var trainingCandidates = genericRepository.Get<TrainingCandidate>(x => 
                    x.CandidateId == user.Id).ToList();

                return GetClassesDetails(trainingCandidates);
            }
            case Constants.Roles.Client:
            {
                if (user.OrganizationId is null) 
                    throw new NotFoundException("The following user has not been registered as a client administrator.");
                
                var candidates = genericRepository.Get<User>(x => 
                    x.OrganizationId == user.OrganizationId).ToList();
            
                var trainingCandidates = genericRepository.Get<TrainingCandidate>(x => 
                    candidates.Select(z => z.Id).Contains(x.CandidateId)).ToList();   
            
                return GetClassesDetails(trainingCandidates);
            }
            case Constants.Roles.Trainer:
            {
                var classTrainers = genericRepository.Get<ClassTrainer>(x => 
                    x.TrainerId == user.Id).ToList();
            
                var classes = genericRepository.Get<Class>(x => classTrainers.Select(z => 
                        z.ClassId).Contains(x.Id)).ToList();
            
                return GetClassesDetails(classes);
            }
        }

        return [];
    }

    private List<GetAllClassesDto> GetClassesDetails(List<TrainingCandidate> trainingCandidates)
    {
        var trainingIds = trainingCandidates.Select(z => z.TrainingId).ToList();
        
        var classes = genericRepository.Get<Class>(x => trainingIds.Contains(x.TrainingId)).ToList();

        return GetClassesDetails(classes);
    }
    
    private List<GetAllClassesDto> GetClassesDetails(List<Class> classes)
    {
        return classes.Select(x => new GetAllClassesDto()
        {
            ClassDates = x.Date.ToFormattedDate(),
            Status = x.Date >= DateOnly.FromDateTime(DateTime.Now) ? Constants.Schedule.ScheduledAction : Constants.Schedule.CompletedAction
        }).ToList();
    }
    #endregion

    #region Private Methods
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
    #endregion
}