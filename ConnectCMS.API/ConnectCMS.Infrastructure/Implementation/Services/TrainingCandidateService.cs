using CMSTrain.Helper;
using CMSTrain.Domain.Common;
using System.Linq.Expressions;
using CMSTrain.Domain.Entities;
using CMSTrain.Application.DTOs.Count;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Common.User;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Training;
using CMSTrain.Application.DTOs.Candidate;
using CMSTrain.Application.DTOs.Organization;
using CMSTrain.Application.DTOs.ClassTrainers;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.TrainingCandidate;
using CMSTrain.Application.DTOs.ClientOrganization;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class TrainingCandidateService(IGenericRepository genericRepository, ICurrentUserService userService)
    : ITrainingCandidateService
{
    public TrainingCandidateAssignmentDetailsDto GetTrainingCandidateAssignmentDetails(Guid trainingCandidateId)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following training candidate request could not be found.");

        var training = genericRepository.GetById<Training>(trainingCandidate.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        return new TrainingCandidateAssignmentDetailsDto()
        {
            TrainingCandidateId = trainingCandidate.Id,
            TrainingId = training.Id,
            CandidateId = candidate.Id,
            RequestedDate = trainingCandidate.RequestedDate.ToFormattedDateTime(),
            ActionDate = trainingCandidate.ActionDate?.ToFormattedDateTime(),
            Remarks = trainingCandidate.Remarks,
            IsSelfRegistered = trainingCandidate.IsSelfRequested,
            IsClientRequestRegistered = trainingCandidate.IsOrganizationRequested,
            IsAdminRegistered = trainingCandidate.IsAdminRequested,
            IsApproved = trainingCandidate.IsApproved,
            IsActionCompleted = trainingCandidate.IsActionCompleted,
            OrganizationId = candidate.OrganizationId
        };
    }

    public GetAllTrainingRequestsForAdmin GetApprovedTrainingCandidateAssignmentDetails(Guid trainingCandidateId)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been registered to the respective training.");

        var training = genericRepository.GetById<Training>(trainingCandidate.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var organization = candidate.OrganizationId != null
            ? genericRepository.GetById<Organization>(candidate.OrganizationId)
            : null;

        var trainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)
                             ?? throw new NotFoundException("The following training format could not be found.");

        return new GetAllTrainingRequestsForAdmin()
        {
            TrainingCandidateId = trainingCandidate.Id,
            IsSelfRequested = trainingCandidate.IsSelfRequested,
            IsOrganizationRequested = trainingCandidate.IsOrganizationRequested,
            IsAdminRequested = trainingCandidate.IsAdminRequested,
            RequestedDate = trainingCandidate.RequestedDate.ToFormattedDateTime(),
            ActionDate = trainingCandidate.ActionDate?.ToFormattedDateTime(),
            Remarks = trainingCandidate.Remarks,
            Action = Constants.RequestAction.AcceptedAction,
            CandidateDetails = new GetCandidateDetailsDto()
            {
                Id = candidate.Id,
                Name = candidate.Name,
                EmailAddress = candidate.Email ?? "",
                PhoneNumber = candidate.PhoneNumber ?? "",
                ImageUrl = candidate.ImageURL,
                Gender = candidate.Gender.ToString(),
                DesignationId = candidate.DesignationId,
                Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null,
                Organization = organization == null
                    ? null
                    : new GetOrganizationDto()
                    {
                        Id = organization.Id,
                        Name = organization.Name,
                        Description = organization.Description,
                        ImageUrl = organization.ImageUrl,
                        Address = organization.Address,
                        IsActive = organization.IsActive,
                    }
            },
            TrainingDetails = new GetTrainingDto()
            {
                Id = training.Id,
                Title = training.Title,
                Description = training.Description,
                ImageUrl = training.ImageUrl,
                StartDate = training.StartDate.ToFormattedDate(),
                EndDate = training.EndDate.ToFormattedDate(),
                TrainingFormatId = trainingFormat.Id,
                TrainingFormat = trainingFormat.Name,
                IsActive = training.IsActive,
                LocationDetails = training.LocationDetails,
                Latitude = training.Latitude ?? 0m,
                Longitude = training.Longitude ?? 0m,
                AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
            }
        };
    }

    public TrainingCandidateAssignmentDetailsDto GetTrainingCandidateAssignmentDetailsForTraining(Guid trainingId)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
            x.TrainingId == training.Id && x.CandidateId == candidate.Id && x.IsApproved);

        if (trainingCandidate is null)
            return new TrainingCandidateAssignmentDetailsDto();

        return new TrainingCandidateAssignmentDetailsDto()
        {
            TrainingCandidateId = trainingCandidate.Id,
            TrainingId = training.Id,
            CandidateId = candidate.Id,
            RequestedDate = trainingCandidate.RequestedDate.ToFormattedDateTime(),
            ActionDate = trainingCandidate.ActionDate?.ToFormattedDateTime(),
            Remarks = trainingCandidate.Remarks,
            IsSelfRegistered = trainingCandidate.IsSelfRequested,
            IsClientRequestRegistered = trainingCandidate.IsOrganizationRequested,
            IsAdminRegistered = trainingCandidate.IsAdminRequested,
            IsApproved = trainingCandidate.IsApproved,
            IsActionCompleted = trainingCandidate.IsActionCompleted,
            OrganizationId = trainingCandidate.IsOrganizationRequested ? candidate.OrganizationId : null
        };
    }

    public void SelfCandidateAssignment(SelfCandidateAssignmentDto assignment)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var training = genericRepository.GetById<Training>(assignment.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var existingTraining =
            genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                x.TrainingId == training.Id && x.CandidateId == candidate.Id);

        if (existingTraining != null)
            throw new BadRequestException("Your request could not be validated",
                ["You have already requested for the following training before."]);

        var trainingCandidate = new TrainingCandidate()
        {
            TrainingId = training.Id,
            CandidateId = candidate.Id,
            RequestedDate = DateTime.UtcNow,
            IsSelfRequested = true,
            IsOrganizationRequested = false
        };

        genericRepository.Insert(trainingCandidate);
    }

    public void ClientCandidateAssignment(ClientCandidateAssignmentDto assignment)
    {
        var clientId = userService.GetUserId;

        var client = genericRepository.GetById<User>(clientId)
                     ?? throw new NotFoundException("The following client has not been registered to our system.");

        var training = genericRepository.GetById<Training>(assignment.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var clientUsers =
            genericRepository.Get<User>(x =>
                x.OrganizationId == client.OrganizationId && x.IsActive && x.Id != client.Id).ToList();

        var existingTraining =
            genericRepository.Get<TrainingCandidate>(x =>
                x.TrainingId == training.Id && clientUsers.Select(z => z.Id).Contains(x.CandidateId)).ToList();

        foreach (var candidateId in assignment.CandidateIds)
        {
            if (existingTraining.Select(x => x.CandidateId).Contains(candidateId))
                continue;

            var trainingCandidate = new TrainingCandidate()
            {
                TrainingId = training.Id,
                CandidateId = candidateId,
                RequestedDate = DateTime.UtcNow,
                IsSelfRequested = false,
                IsOrganizationRequested = true,
                IsOrganizationHandled = true,
                IsActionCompleted = true,
                IsApproved = true,
                ActionDate = DateTime.UtcNow,
                Remarks = "Client Requested"
            };

            genericRepository.Insert(trainingCandidate);
        }
    }

    public void AdminCandidateAssignment(AssignCandidatesDto assignCandidates)
    {
        var training = genericRepository.GetById<Training>(assignCandidates.TrainingId)
            ?? throw new NotFoundException("The following training could not be found.");

        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x => 
            x.TrainingId == training.Id).ToList();
        
        foreach (var candidateId in assignCandidates.CandidateIds)
        {
            var candidate = genericRepository.GetById<User>(candidateId)
                ?? throw new NotFoundException("The following candidate has not been registered to our system.");
            
            if (trainingCandidates.Any(x => x.CandidateId == candidate.Id)) continue;

            var trainingCandidate = new TrainingCandidate()
            {
                TrainingId = training.Id,
                CandidateId = candidateId,
                RequestedDate = DateTime.UtcNow,
                IsSelfRequested = false,
                IsOrganizationRequested = false,
                IsOrganizationHandled = false,
                IsActionCompleted = true,
                IsApproved = true,
                IsAdminRequested = true,
                ActionDate = DateTime.UtcNow,
                Remarks = "Admin Requested"
            };

            genericRepository.Insert(trainingCandidate);
        }
    }

    public void ApprovalRejectTrainingCandidateRequest(ApproveRejectRequestDto approveRejectRequest)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(approveRejectRequest.TrainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following training candidate request could not be found.");

        trainingCandidate.IsActionCompleted = true;
        trainingCandidate.ActionDate = DateTime.UtcNow;
        trainingCandidate.Remarks = approveRejectRequest.Remarks;
        trainingCandidate.IsApproved = approveRejectRequest.IsApproved;

        genericRepository.Update(trainingCandidate);
    }

    public void RemoveCandidateFromTraining(Guid trainingCandidateId)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been assigned to the respective training.");

        var subordinates = genericRepository.Get<Subordinate>(x => 
            x.TrainingCandidateId == trainingCandidate.Id).ToList();
        
        var certifications = genericRepository.Get<Certificate>(x => 
            x.TrainingCandidateId == trainingCandidate.Id).ToList();

        if (subordinates.Any())
        {
            genericRepository.RemoveMultipleEntity(subordinates);
        }

        if (certifications.Any())
        {
            genericRepository.RemoveMultipleEntity(certifications);
        }
        
        genericRepository.Delete(trainingCandidate);
    }
    
    public void HandleOrganizationCandidatesPermission(Guid trainingCandidateId)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following training candidate request could not be found.");

        if (!trainingCandidate.IsOrganizationRequested)
        {
            throw new BadRequestException("The following candidate's action could not be updated.",
                ["Only the permissions can be granted to organizational candidates"]);
        }

        trainingCandidate.IsOrganizationHandled = !trainingCandidate.IsOrganizationHandled;

        genericRepository.Update(trainingCandidate);
    }

    public void CancelTrainingRequest(Guid trainingCandidateId)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following training candidate request could not be found.");

        genericRepository.Delete(trainingCandidate);
    }

    public GetTrainingRequestsCount GetTrainingRequestsCount(Guid? trainingId = null)
    {
        var training = trainingId == null
            ? null
            : genericRepository.GetById<Training>(trainingId)
              ?? throw new NotFoundException("The following training could not be found.");

        var trainingRequests = genericRepository.Get<TrainingCandidate>(x =>
            training == null || x.TrainingId == training.Id).ToList();

        var startOfThisWeek = DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
        var endOfThisWeek = startOfThisWeek.AddDays(5).AddSeconds(-1);

        var startOfLastWeek = startOfThisWeek.AddDays(-7);
        var endOfLastWeek = startOfLastWeek.AddDays(5).AddSeconds(-1);

        var thisWeekTrainingRequests = genericRepository
            .Get<TrainingCandidate>()
            .Where(tc => tc.RequestedDate.Date >= startOfThisWeek.Date && tc.RequestedDate.Date <= endOfThisWeek.Date)
            .ToList();

        var lastWeekTrainingRequests = genericRepository
            .Get<TrainingCandidate>()
            .Where(tc => tc.RequestedDate.Date >= startOfLastWeek.Date && tc.RequestedDate.Date <= endOfLastWeek.Date)
            .ToList();

        var thisWeekAcceptedRequests = thisWeekTrainingRequests.Where(x => x.IsApproved);
        var lastWeekAcceptedRequests = lastWeekTrainingRequests.Where(x => x.IsApproved);

        var thisWeekRejectedRequests =
            thisWeekTrainingRequests.Where(x => x is { IsActionCompleted: true, IsApproved: false });
        var lastWeekRejectedRequests =
            lastWeekTrainingRequests.Where(x => x is { IsActionCompleted: true, IsApproved: false });

        var thisWeekPendingRequests = thisWeekTrainingRequests.Where(x => !x.IsActionCompleted);
        var lastWeekPendingRequests = lastWeekTrainingRequests.Where(x => !x.IsActionCompleted);

        var totalRequestsCount = trainingRequests.Count == 0 ? 1 : trainingRequests.Count;
        var totalAcceptedRequestsCount =
            trainingRequests.Any(x => x.IsApproved) ? trainingRequests.Count(x => x.IsApproved) : 1;
        var totalPendingRequestsCount = trainingRequests.Any(x => !x.IsActionCompleted)
            ? trainingRequests.Count(x => !x.IsActionCompleted)
            : 1;
        var totalRejectedRequestsCount = trainingRequests.Any(x => x is { IsActionCompleted: true, IsApproved: false })
            ? trainingRequests.Count(x => x is { IsActionCompleted: true, IsApproved: false })
            : 1;

        return new GetTrainingRequestsCount()
        {
            TotalRequests = trainingRequests.Count,
            TotalAcceptedRequests = trainingRequests.Count(x => x.IsApproved),
            TotalPendingRequests = trainingRequests.Count(x => !x.IsActionCompleted),
            TotalRejectedRequests = trainingRequests.Count(x => x is { IsActionCompleted: true, IsApproved: false }),
            TotalRequestsGrowthFromLastWeek =
                Math.Round(
                    (decimal)(thisWeekTrainingRequests.Count - lastWeekTrainingRequests.Count) * 100 /
                    totalRequestsCount, 2),
            TotalAcceptedRequestsGrowthFromLastWeek = Math.Round(
                (decimal)(thisWeekAcceptedRequests.Count() - lastWeekAcceptedRequests.Count()) * 100 /
                totalAcceptedRequestsCount, 2),
            TotalPendingRequestsGrowthFromLastWeek =
                Math.Round(
                    (decimal)(thisWeekPendingRequests.Count() - lastWeekPendingRequests.Count()) * 100 /
                    totalPendingRequestsCount, 2),
            TotalRejectedRequestsGrowthFromLastWeek = Math.Round(
                (decimal)(thisWeekRejectedRequests.Count() - lastWeekRejectedRequests.Count()) * 100 /
                totalRejectedRequestsCount, 2)
        };
    }

    public ApprovalMatrixCountDto GetApprovalMatrixCount(Guid? trainingId = null)
    {
        var training = trainingId == null
            ? null
            : genericRepository.GetById<Training>(trainingId)
              ?? throw new NotFoundException("The following training could not be found.");

        var pendingCount = genericRepository.GetCount<TrainingCandidate>(x =>
            (training == null || x.TrainingId == training.Id) && !x.IsActionCompleted);

        var approvedCount = genericRepository.GetCount<TrainingCandidate>(x =>
            (training == null || x.TrainingId == training.Id) && x.IsActionCompleted && x.IsApproved);

        var rejectedCount = genericRepository.GetCount<TrainingCandidate>(x =>
            (training == null || x.TrainingId == training.Id) && x.IsActionCompleted && !x.IsApproved);

        return new ApprovalMatrixCountDto()
        {
            PendingCount = pendingCount,
            RejectedCount = rejectedCount,
            ApprovedCount = approvedCount
        };
    }

    public List<GetAllTrainingRequestsForAdmin> GetAllTrainingRequestsForAdmin(int action, int pageNumber, int pageSize,
        out int rowCount, string? search = null, Guid? trainingId = null)
    {
        var trainingModel = trainingId == null
            ? null
            : genericRepository.GetById<Training>(trainingId)
              ?? throw new NotFoundException("The following training could not be found.");

        var candidateTrainings = action switch
        {
            Constants.RequestAction.Pending => genericRepository
                .GetPagedResult<TrainingCandidate>(pageNumber, pageSize, out rowCount,
                    x => (trainingModel == null || x.TrainingId == trainingModel.Id) && !x.IsActionCompleted)
                .ToList(),
            Constants.RequestAction.Accepted => genericRepository
                .GetPagedResult<TrainingCandidate>(pageNumber, pageSize, out rowCount,
                    x => (trainingModel == null || x.TrainingId == trainingModel.Id) && x.IsActionCompleted &&
                         x.IsApproved)
                .ToList(),
            Constants.RequestAction.Rejected => genericRepository
                .GetPagedResult<TrainingCandidate>(pageNumber, pageSize, out rowCount,
                    x => (trainingModel == null || x.TrainingId == trainingModel.Id) && x.IsActionCompleted &&
                         !x.IsApproved)
                .ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };

        var candidateRequests = new List<GetAllTrainingRequestsForAdmin>();

        foreach (var candidateTraining in candidateTrainings)
        {
            var candidate = genericRepository.GetById<User>(candidateTraining.CandidateId)
                            ?? throw new NotFoundException(
                                "The following candidate has not been registered to our system.");

            var organization = candidate.OrganizationId != null
                ? genericRepository.GetById<Organization>(candidate.OrganizationId)
                : null;

            if (!string.IsNullOrEmpty(search) && !candidate.Name.ToLower().Contains(search.ToLower())) continue;

            var training = genericRepository.GetById<Training>(candidateTraining.TrainingId)
                           ?? throw new NotFoundException("The following training could not be found.");

            var trainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)
                                 ?? throw new NotFoundException("The following training format could not be found.");

            candidateRequests.Add(new GetAllTrainingRequestsForAdmin()
            {
                TrainingCandidateId = candidateTraining.Id,
                IsSelfRequested = candidateTraining.IsSelfRequested,
                IsOrganizationRequested = candidateTraining.IsOrganizationRequested,
                IsAdminRequested = candidateTraining.IsAdminRequested,
                RequestedDate = candidateTraining.RequestedDate.ToFormattedDateTime(),
                ActionDate = candidateTraining.ActionDate?.ToFormattedDateTime(),
                Remarks = candidateTraining.Remarks,
                Action = action switch
                {
                    Constants.RequestAction.Pending => Constants.RequestAction.PendingAction,
                    Constants.RequestAction.Accepted => Constants.RequestAction.AcceptedAction,
                    Constants.RequestAction.Rejected => Constants.RequestAction.RejectedAction,
                    _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
                },
                CandidateDetails = new GetCandidateDetailsDto()
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    EmailAddress = candidate.Email ?? "",
                    PhoneNumber = candidate.PhoneNumber ?? "",
                    ImageUrl = candidate.ImageURL,
                    Gender = candidate.Gender.ToString(),
                    DesignationId = candidate.DesignationId,
                    Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null,
                    Organization = organization == null
                        ? null
                        : new GetOrganizationDto()
                        {
                            Id = organization.Id,
                            Name = organization.Name,
                            Description = organization.Description,
                            ImageUrl = organization.ImageUrl,
                            Address = organization.Address,
                            IsActive = organization.IsActive,
                        }
                },
                TrainingDetails = new GetTrainingDto()
                {
                    Id = training.Id,
                    Title = training.Title,
                    Description = training.Description,
                    ImageUrl = training.ImageUrl,
                    StartDate = training.StartDate.ToFormattedDate(),
                    EndDate = training.EndDate.ToFormattedDate(),
                    TrainingFormatId = trainingFormat.Id,
                    TrainingFormat = trainingFormat.Name,
                    Latitude = training.Latitude ?? 0m,
                    Longitude = training.Longitude ?? 0m,
                    IsActive = training.IsActive,
                    LocationDetails = training.LocationDetails,
                    AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
                }
            });
        }

        return candidateRequests;
    }

    public List<GetAllTrainingRequestsForAdmin> GetAllTrainingRequestsForAdmin(int action, string? search = null,
        Guid? trainingId = null)
    {
        var trainingModel = trainingId == null
            ? null
            : genericRepository.GetById<Training>(trainingId)
              ?? throw new NotFoundException("The following training could not be found.");

        var candidateTrainings = action switch
        {
            Constants.RequestAction.Pending => genericRepository
                .Get<TrainingCandidate>(x =>
                    (trainingModel == null || x.TrainingId == trainingModel.Id) && !x.IsActionCompleted)
                .ToList(),
            Constants.RequestAction.Accepted => genericRepository
                .Get<TrainingCandidate>(x =>
                    (trainingModel == null || x.TrainingId == trainingModel.Id) && x.IsActionCompleted && x.IsApproved)
                .ToList(),
            Constants.RequestAction.Rejected => genericRepository
                .Get<TrainingCandidate>(x =>
                    (trainingModel == null || x.TrainingId == trainingModel.Id) && x.IsActionCompleted && !x.IsApproved)
                .ToList(),
            _ => []
        };

        var candidateRequests = new List<GetAllTrainingRequestsForAdmin>();

        foreach (var candidateTraining in candidateTrainings)
        {
            var candidate = genericRepository.GetById<User>(candidateTraining.CandidateId)
                            ?? throw new NotFoundException(
                                "The following candidate has not been registered to our system.");

            var organization = candidate.OrganizationId != null
                ? genericRepository.GetById<Organization>(candidate.OrganizationId)
                : null;

            if (!string.IsNullOrEmpty(search) && !candidate.Name.ToLower().Contains(search.ToLower())) continue;

            var training = genericRepository.GetById<Training>(candidateTraining.TrainingId)
                           ?? throw new NotFoundException("The following training could not be found.");

            var trainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)
                                 ?? throw new NotFoundException("The following training format could not be found.");

            candidateRequests.Add(new GetAllTrainingRequestsForAdmin()
            {
                TrainingCandidateId = candidateTraining.Id,
                IsSelfRequested = candidateTraining.IsSelfRequested,
                IsOrganizationRequested = candidateTraining.IsOrganizationRequested,
                IsAdminRequested = candidateTraining.IsAdminRequested,
                RequestedDate = candidateTraining.RequestedDate.ToFormattedDateTime(),
                ActionDate = candidateTraining.ActionDate?.ToFormattedDateTime(),
                Remarks = candidateTraining.Remarks,
                Action = action switch
                {
                    Constants.RequestAction.Pending => Constants.RequestAction.PendingAction,
                    Constants.RequestAction.Accepted => Constants.RequestAction.AcceptedAction,
                    _ => Constants.RequestAction.RejectedAction
                },
                CandidateDetails = new GetCandidateDetailsDto()
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    EmailAddress = candidate.Email ?? "",
                    PhoneNumber = candidate.PhoneNumber ?? "",
                    ImageUrl = candidate.ImageURL,
                    Gender = candidate.Gender.ToString(),
                    DesignationId = candidate.DesignationId,
                    Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null,
                    Organization = organization == null
                        ? null
                        : new GetOrganizationDto()
                        {
                            Id = organization.Id,
                            Name = organization.Name,
                            Description = organization.Description,
                            ImageUrl = organization.ImageUrl,
                            Address = organization.Address,
                            IsActive = organization.IsActive,
                        }
                },
                TrainingDetails = new GetTrainingDto()
                {
                    Id = training.Id,
                    Title = training.Title,
                    Description = training.Description,
                    ImageUrl = training.ImageUrl,
                    StartDate = training.StartDate.ToFormattedDate(),
                    EndDate = training.EndDate.ToFormattedDate(),
                    TrainingFormatId = trainingFormat.Id,
                    TrainingFormat = trainingFormat.Name,
                    Latitude = training.Latitude ?? 0m,
                    Longitude = training.Longitude ?? 0m,
                    IsActive = training.IsActive,
                    LocationDetails = training.LocationDetails,
                    AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
                }
            });
        }

        return candidateRequests;
    }

    public List<GetAllTrainingRequestsForCandidate> GetAllTrainingRequestsForCandidate(int action, int pageNumber,
        int pageSize, out int rowCount, string? search = null)
    {
        var userId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(userId);

        if (candidate == null)
            throw new NotFoundException("The following candidate has not been registered to our system.");

        var organization = candidate.OrganizationId != null
            ? genericRepository.GetById<Organization>(candidate.OrganizationId) ??
              throw new NotFoundException("The following organization has not been registered to our system.")
            : null;

        var candidateTrainings = action switch
        {
            Constants.RequestAction.Pending => genericRepository
                .GetPagedResult<TrainingCandidate>(pageNumber, pageSize, out rowCount,
                    x => x.CandidateId == candidate.Id && !x.IsActionCompleted)
                .ToList(),
            Constants.RequestAction.Accepted => genericRepository
                .GetPagedResult<TrainingCandidate>(pageNumber, pageSize, out rowCount,
                    x => x.CandidateId == candidate.Id && x.IsActionCompleted && x.IsApproved)
                .ToList(),
            Constants.RequestAction.Rejected => genericRepository
                .GetPagedResult<TrainingCandidate>(pageNumber, pageSize, out rowCount,
                    x => x.CandidateId == candidate.Id && x.IsActionCompleted && !x.IsApproved)
                .ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };

        var trainingRequests = new List<GetAllTrainingRequestsForCandidate>();

        foreach (var candidateTraining in candidateTrainings)
        {
            var training = genericRepository.GetById<Training>(candidateTraining.TrainingId)
                           ?? throw new NotFoundException("The following training could not be found.");

            var trainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)
                                 ?? throw new NotFoundException("The following training format could not be found.");

            if (!string.IsNullOrEmpty(search) && !training.Title.ToLower().Contains(search.ToLower())) continue;

            trainingRequests.Add(new GetAllTrainingRequestsForCandidate()
            {
                TrainingCandidateId = candidateTraining.Id,
                IsSelfRequested = candidateTraining.IsSelfRequested,
                IsOrganizationRequested = candidateTraining.IsOrganizationRequested,
                IsAdminRequested = candidateTraining.IsAdminRequested,
                RequestedDate = candidateTraining.RequestedDate.ToFormattedDateTime(),
                ActionDate = candidateTraining.ActionDate?.ToFormattedDateTime(),
                Remarks = candidateTraining.Remarks,
                OrganizationId = candidateTraining.IsOrganizationRequested ? organization?.Id : null,
                Organization = candidateTraining.IsOrganizationRequested ? organization?.Name : null,
                Action = action switch
                {
                    Constants.RequestAction.Pending => Constants.RequestAction.PendingAction,
                    Constants.RequestAction.Accepted => Constants.RequestAction.AcceptedAction,
                    Constants.RequestAction.Rejected => Constants.RequestAction.RejectedAction,
                    _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
                },
                TrainingDetails = new GetTrainingDto()
                {
                    Id = training.Id,
                    Title = training.Title,
                    Description = training.Description,
                    IsActive = training.IsActive,
                    EndDate = training.EndDate.ToFormattedDate(),
                    StartDate = training.StartDate.ToFormattedDate(),
                    LocationDetails = training.LocationDetails,
                    ImageUrl = training.ImageUrl,
                    Latitude = training.Latitude ?? 0m,
                    Longitude = training.Longitude ?? 0m,
                    TrainingFormatId = trainingFormat.Id,
                    TrainingFormat = trainingFormat.Name,
                    AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
                }
            });
        }

        return trainingRequests;
    }

    public List<GetAllTrainingRequestsForCandidate> GetAllTrainingRequestsForCandidate(int action,
        string? search = null)
    {
        var userId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(userId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var organization = candidate.OrganizationId != null
            ? genericRepository.GetById<Organization>(candidate.OrganizationId) ??
              throw new NotFoundException("The following organization has not been registered to our system.")
            : null;

        var candidateTrainings = action switch
        {
            Constants.RequestAction.Pending => genericRepository
                .Get<TrainingCandidate>(x => x.CandidateId == candidate.Id && !x.IsActionCompleted)
                .ToList(),
            Constants.RequestAction.Accepted => genericRepository
                .Get<TrainingCandidate>(x => x.CandidateId == candidate.Id && x.IsActionCompleted && x.IsApproved)
                .ToList(),
            Constants.RequestAction.Rejected => genericRepository
                .Get<TrainingCandidate>(x => x.CandidateId == candidate.Id && x.IsActionCompleted && !x.IsApproved)
                .ToList(),
            _ => []
        };

        var trainingRequests = new List<GetAllTrainingRequestsForCandidate>();

        foreach (var candidateTraining in candidateTrainings)
        {
            var training = genericRepository.GetById<Training>(candidateTraining.TrainingId)
                           ?? throw new NotFoundException("The following training could not be found.");

            var trainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)
                                 ?? throw new NotFoundException("The following training format could not be found.");

            if (!string.IsNullOrEmpty(search) && !training.Title.ToLower().Contains(search.ToLower())) continue;

            trainingRequests.Add(new GetAllTrainingRequestsForCandidate()
            {
                TrainingCandidateId = candidateTraining.Id,
                IsSelfRequested = candidateTraining.IsSelfRequested,
                IsOrganizationRequested = candidateTraining.IsOrganizationRequested,
                IsAdminRequested = candidateTraining.IsAdminRequested,
                RequestedDate = candidateTraining.RequestedDate.ToFormattedDateTime(),
                ActionDate = candidateTraining.ActionDate?.ToFormattedDateTime(),
                Remarks = candidateTraining.Remarks,
                OrganizationId = candidateTraining.IsOrganizationRequested ? organization?.Id : null,
                Organization = candidateTraining.IsOrganizationRequested ? organization?.Name : null,
                Action = action switch
                {
                    Constants.RequestAction.Pending => Constants.RequestAction.PendingAction,
                    Constants.RequestAction.Accepted => Constants.RequestAction.AcceptedAction,
                    _ => Constants.RequestAction.RejectedAction
                },
                TrainingDetails = new GetTrainingDto()
                {
                    Id = training.Id,
                    Title = training.Title,
                    Description = training.Description,
                    IsActive = training.IsActive,
                    Latitude = training.Latitude ?? 0m,
                    Longitude = training.Longitude ?? 0m,
                    EndDate = training.EndDate.ToFormattedDate(),
                    StartDate = training.StartDate.ToFormattedDate(),
                    LocationDetails = training.LocationDetails,
                    ImageUrl = training.ImageUrl,
                    TrainingFormatId = trainingFormat.Id,
                    TrainingFormat = trainingFormat.Name,
                    AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
                }
            });
        }

        return trainingRequests;
    }
    
    public List<GetApprovedCandidateDetailsDto> GetAllApprovedCandidatesForTraining(Guid trainingId, int pageNumber,
        int pageSize, out int rowCount, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId) ??
                       throw new NotFoundException("The following training could not be found.");

        var trainingCandidates =
            genericRepository.GetPagedResult<TrainingCandidate>(pageNumber, pageSize, out rowCount, x =>
                x.TrainingId == training.Id && x.IsActionCompleted && x.IsApproved).ToList();

        var candidateIds = trainingCandidates.Select(x => x.CandidateId).ToList();

        var candidates = new List<GetApprovedCandidateDetailsDto>();

        foreach (var candidateId in candidateIds)
        {
            var candidate = genericRepository.GetById<User>(candidateId)
                            ?? throw new NotFoundException(
                                "The following candidate has not been registered to our system.");

            if (candidate.Email != null && !string.IsNullOrEmpty(search) && (!candidate.Name.ToLower().Contains(search.ToLower()) ||
                                                                             !candidate.Email.ToLower().Contains(search.ToLower()))) continue;

            var organization = candidate.OrganizationId == null
                ? null
                : genericRepository.GetById<Organization>(candidate.OrganizationId) ??
                  throw new NotFoundException("The following organization could not be found.");

            var trainingCandidate = trainingCandidates.FirstOrDefault(x =>
                                        x.CandidateId == candidate.Id && x.TrainingId == training.Id)
                                    ?? throw new NotFoundException(
                                        "The respective candidate has not been approved to the following training.");

            candidates.Add(new GetApprovedCandidateDetailsDto()
            {
                TrainingCandidateId = trainingCandidate.Id,
                ApprovedDate = trainingCandidate.ActionDate?.ToFormattedDateTime(),
                Id = candidate.Id,
                Name = candidate.Name,
                EmailAddress = candidate.Email ?? "",
                PhoneNumber = candidate.PhoneNumber ?? "",
                ImageUrl = candidate.ImageURL,
                RequestedDate = trainingCandidate.RequestedDate.ToFormattedDateTime(),
                ActionDate = trainingCandidate.ActionDate?.ToFormattedDateTime(),
                IsAdminRegistered = trainingCandidate.IsAdminRequested,
                IsSelfRegistered = trainingCandidate.IsSelfRequested,
                IsClientRequestRegistered = trainingCandidate.IsOrganizationRequested,
                Gender = candidate.Gender.ToString(),
                DesignationId = candidate.DesignationId,
                Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)!.Title : null,
                Organization = candidate.OrganizationId == null
                    ? null
                    : new GetOrganizationDto()
                    {
                        Id = organization!.Id,
                        Name = organization.Name,
                        Description = organization.Description,
                        IsActive = organization.IsActive,
                        ImageUrl = organization.ImageUrl,
                        Address = organization.Address
                    }
            });
        }

        return candidates;
    }

    public List<GetCandidateDetailsDto> GetAllUnassignedCandidatesForTraining(Guid trainingId)
    {
        var training = genericRepository.GetById<Training>(trainingId) 
                       ?? throw new NotFoundException("The following training could not be found.");

        var requestedCandidateIds = genericRepository.Get<TrainingCandidate>(x => 
                x.TrainingId == training.Id).Select(x => 
                    x.CandidateId).ToList();

        var candidateRole = genericRepository.GetFirstOrDefault<Role>(x => x.Name == Constants.Roles.Candidate)
            ?? throw new NotFoundException("The following role could not be found.");

        var candidateUserRoles = genericRepository.Get<UserRoles>(x => 
            x.RoleId == candidateRole.Id).ToList();

        var unassignedCandidates = genericRepository.Get<User>(x => 
                candidateUserRoles.Select(z => 
                    z.UserId).Contains(x.Id) && !requestedCandidateIds.Contains(x.Id) && x.IsActive).ToList();

        return (from candidate in unassignedCandidates
                let organization = candidate.OrganizationId == null
                ? null
                : genericRepository.GetById<Organization>(candidate.OrganizationId) 
                  ?? throw new NotFoundException("The organization could not be found for this candidate.")
            select new GetCandidateDetailsDto()
            {
                Id = candidate.Id,
                Name = candidate.Name,
                EmailAddress = candidate.Email ?? "",
                PhoneNumber = candidate.PhoneNumber ?? "",
                ImageUrl = candidate.ImageURL,
                Gender = candidate.Gender.ToString(),
                DesignationId = candidate.DesignationId,
                Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null,
                Organization = candidate.OrganizationId == null
                    ? null
                    : new GetOrganizationDto()
                    {
                        Id = organization!.Id,
                        Name = organization.Name,
                        Description = organization.Description,
                        IsActive = organization.IsActive,
                        ImageUrl = organization.ImageUrl ??  "",
                        Address = organization.Address
                    }
            }).ToList();
    }

    public List<GetApprovedCandidateDetailsDto> GetAllApprovedCandidatesForTraining(Guid trainingId)
    {
        var training = genericRepository.GetById<Training>(trainingId) ??
                       throw new NotFoundException("The following training could not be found.");

        var trainingCandidates =
            genericRepository.Get<TrainingCandidate>(x =>
                x.TrainingId == training.Id && x.IsActionCompleted && x.IsApproved).ToList();

        var candidateIds = trainingCandidates.Select(x => x.CandidateId).ToList();

        var candidates = new List<GetApprovedCandidateDetailsDto>();

        foreach (var candidateId in candidateIds)
        {
            var candidate = genericRepository.GetById<User>(candidateId) ??
                            throw new NotFoundException(
                                "The following candidate has not been registered to our system.");

            var organization = candidate.OrganizationId == null
                ? null
                : genericRepository.GetById<Organization>(candidate.OrganizationId) ??
                  throw new NotFoundException("The following organization could not be found.");

            var trainingCandidate = trainingCandidates.FirstOrDefault(x =>
                                        x.CandidateId == candidate.Id && x.TrainingId == training.Id)
                                    ?? throw new NotFoundException(
                                        "The respective candidate has not been approved to the following training.");

            candidates.Add(new GetApprovedCandidateDetailsDto()
            {
                TrainingCandidateId = trainingCandidate.Id,
                ApprovedDate = trainingCandidate.ActionDate?.ToFormattedDateTime(),
                Id = candidate.Id,
                Name = candidate.Name,
                EmailAddress = candidate.Email ?? "",
                PhoneNumber = candidate.PhoneNumber ?? "",
                ImageUrl = candidate.ImageURL,
                Gender = candidate.Gender.ToString(),
                DesignationId = candidate.DesignationId,
                Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null,
                Organization = candidate.OrganizationId == null
                    ? null
                    : new GetOrganizationDto()
                    {
                        Id = organization!.Id,
                        Name = organization.Name,
                        Description = organization.Description,
                        IsActive = organization.IsActive,
                        ImageUrl = organization.ImageUrl,
                        Address = organization.Address
                    }
            });
        }

        return candidates;
    }

    public List<GetApprovedCandidateDetailsDto> GetAllColleagueCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var candidateId = userService.GetUserId;

        var userRole = userService.GetUserRole;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var candidates = new List<GetApprovedCandidateDetailsDto>();

        if (userRole == Constants.Roles.Candidate)
        {
            var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                                        x.CandidateId == candidate.Id && x.TrainingId == training.Id)
                                    ?? throw new NotFoundException(
                                        "The respective candidate has not been approved to the following training.");

            if (candidate.Email != null && (string.IsNullOrEmpty(search) || candidate.Name.ToLower().Contains(search.ToLower()) || candidate.Email.ToLower().Contains(search.ToLower())))
            {
                candidates.Add(new GetApprovedCandidateDetailsDto()
                {
                    TrainingCandidateId = trainingCandidate.Id,
                    ApprovedDate = trainingCandidate.ActionDate?.ToFormattedDateTime(),
                    Id = candidate.Id,
                    Name = candidate.Name,
                    EmailAddress = candidate.Email ?? "",
                    PhoneNumber = candidate.PhoneNumber ?? "",
                    ImageUrl = candidate.ImageURL,
                    Organization = null,
                    Gender = candidate.Gender.ToString(),
                    DesignationId = candidate.DesignationId,
                    Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null
                });
            }
        }

        var trainingCandidates =
            genericRepository.GetPagedResult<TrainingCandidate>(pageNumber, pageSize, out rowCount, x =>
                    x.TrainingId == training.Id && x.CandidateId != candidate.Id && x.IsActionCompleted && x.IsApproved)
                .ToList();

        var candidateIds = trainingCandidates.Select(x => x.CandidateId).ToList();

        foreach (var candidateIdentifier in candidateIds)
        {
            var candidateUser = genericRepository.GetById<User>(candidateIdentifier) ??
                                throw new NotFoundException(
                                    "The following candidate has not been registered to our system.");

            if (candidateUser.Email != null && !string.IsNullOrEmpty(search) && !(candidateUser.Name.ToLower().Contains(search.ToLower()) ||
                    candidateUser.Email.ToLower().Contains(search.ToLower()))) continue;

            var organization = candidateUser.OrganizationId == null
                ? null
                : genericRepository.GetById<Organization>(candidateUser.OrganizationId) ??
                  throw new NotFoundException("The following organization could not be found.");

            var colleagueTrainingCandidate = trainingCandidates.FirstOrDefault(x =>
                                                 x.CandidateId == candidateUser.Id && x.TrainingId == training.Id)
                                             ?? throw new NotFoundException(
                                                 "The respective candidate has not been approved to the following training.");

            candidates.Add(new GetApprovedCandidateDetailsDto()
            {
                TrainingCandidateId = colleagueTrainingCandidate.Id,
                ApprovedDate = colleagueTrainingCandidate.ActionDate?.ToFormattedDateTime(),
                Id = candidateUser.Id,
                Name = candidateUser.Name,
                EmailAddress = candidateUser.Email ?? "",
                PhoneNumber = candidateUser.PhoneNumber ?? "",
                ImageUrl = candidateUser.ImageURL,
                Gender = candidate.Gender.ToString(),
                DesignationId = candidate.DesignationId,
                Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null,
                Organization = candidateUser.OrganizationId == null
                    ? null
                    : new GetOrganizationDto()
                    {
                        Id = organization!.Id,
                        Name = organization.Name,
                        Description = organization.Description,
                        IsActive = organization.IsActive,
                        ImageUrl = organization.ImageUrl,
                        Address = organization.Address
                    }
            });
        }

        return candidates;
    }

    public List<GetApprovedCandidateDetailsDto> GetAllColleagueCandidatesForTraining(Guid trainingId, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId) ??
                       throw new NotFoundException("The following training could not be found.");

        var candidateId = userService.GetUserId;

        var userRole = userService.GetUserRole;

        var candidate = genericRepository.GetById<User>(candidateId) ??
                        throw new NotFoundException("The following candidate has not been registered to our system.");

        var candidates = new List<GetApprovedCandidateDetailsDto>();

        if (userRole == Constants.Roles.Candidate)
        {
            var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                                        x.CandidateId == candidate.Id && x.TrainingId == training.Id)
                                    ?? throw new NotFoundException(
                                        "The respective candidate has not been approved to the following training.");
            
            if (candidate.Email != null && (string.IsNullOrEmpty(search) || (candidate.Name.ToLower().Contains(search.ToLower()) || candidate.Email.ToLower().Contains(search.ToLower()))))
            {
                candidates.Add(new GetApprovedCandidateDetailsDto()
                {
                    TrainingCandidateId = trainingCandidate.Id,
                    ApprovedDate = trainingCandidate.ActionDate?.ToFormattedDateTime(),
                    Id = candidate.Id,
                    Name = candidate.Name,
                    EmailAddress = candidate.Email ?? "",
                    PhoneNumber = candidate.PhoneNumber ?? "",
                    ImageUrl = candidate.ImageURL,
                    Gender = candidate.Gender.ToString(),
                    Organization = null,
                    DesignationId = candidate.DesignationId,
                    Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null,
                });
            }
        }

        var trainingCandidates =
            genericRepository.Get<TrainingCandidate>(x =>
                    x.TrainingId == training.Id && x.CandidateId != candidate.Id && x.IsActionCompleted && x.IsApproved)
                .ToList();

        var candidateIds = trainingCandidates.Select(x => x.CandidateId).ToList();

        foreach (var candidateIdentifier in candidateIds)
        {
            var candidateUser = genericRepository.GetById<User>(candidateIdentifier) ??
                                throw new NotFoundException(
                                    "The following candidate has not been registered to our system.");
            
            if (candidateUser.Email != null && !string.IsNullOrEmpty(search) && !(candidateUser.Name.ToLower().Contains(search.ToLower()) || candidateUser.Email.ToLower().Contains(search.ToLower()))) continue;

            var organization = candidateUser.OrganizationId == null
                ? null
                : genericRepository.GetById<Organization>(candidateUser.OrganizationId) ??
                  throw new NotFoundException("The following organization could not be found.");

            var colleagueTrainingCandidate = trainingCandidates.FirstOrDefault(x =>
                                                 x.CandidateId == candidateUser.Id && x.TrainingId == training.Id)
                                             ?? throw new NotFoundException(
                                                 "The respective candidate has not been approved to the following training.");

            candidates.Add(new GetApprovedCandidateDetailsDto()
            {
                TrainingCandidateId = colleagueTrainingCandidate.Id,
                ApprovedDate = colleagueTrainingCandidate.ActionDate?.ToFormattedDateTime(),
                Id = candidateUser.Id,
                Name = candidateUser.Name,
                EmailAddress = candidateUser.Email ?? "",
                PhoneNumber = candidateUser.PhoneNumber ?? "",
                ImageUrl = candidateUser.ImageURL,
                Gender = candidate.Gender.ToString(),
                Organization = candidateUser.OrganizationId == null
                    ? null
                    : new GetOrganizationDto()
                    {
                        Id = organization!.Id,
                        Name = organization.Name,
                        Description = organization.Description,
                        IsActive = organization.IsActive,
                        ImageUrl = organization.ImageUrl,
                        Address = organization.Address
                    },
                DesignationId = candidate.DesignationId,
                Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null,
            });
        }

        return candidates;
    }

    public List<GetApprovedCandidateDetailsDto> GetAllOrganizationalCandidatesForTraining(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var candidateId = userService.GetUserId;

        var userRole = userService.GetUserRole;

        var candidate = genericRepository.GetById<User>(candidateId) ??
                        throw new NotFoundException("The following candidate has not been registered to our system.");

        if (candidate.OrganizationId == null)
            throw new BadRequestException("Organizational Candidates could not be fetched",
                ["The following user does not belong to a particular client organization."]);

        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
            x.CandidateId == candidate.Id && x.TrainingId == training.Id);

        var candidates = new List<GetApprovedCandidateDetailsDto>();

        if (userRole == Constants.Roles.Candidate)
        {
            if (trainingCandidate == null)
            {
                throw new NotFoundException(
                    "The respective candidate has not been approved to the following training.");
            }

            if (candidate.Email != null && (string.IsNullOrEmpty(search) || (candidate.Name.ToLower().Contains(search.ToLower()) || candidate.Email.ToLower().Contains(search.ToLower()))))
            {
                candidates.Add(new GetApprovedCandidateDetailsDto()
                {
                    TrainingCandidateId = trainingCandidate.Id,
                    ApprovedDate = trainingCandidate.ActionDate?.ToFormattedDateTime(),
                    Id = candidate.Id,
                    Name = candidate.Name,
                    EmailAddress = candidate.Email ?? "",
                    PhoneNumber = candidate.PhoneNumber ?? "",
                    ImageUrl = candidate.ImageURL,
                    Organization = null,
                    Gender = candidate.Gender.ToString(),
                    DesignationId = candidate.DesignationId,
                    Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null
                });
            }
        }

        var organizationUsers = genericRepository.GetPagedResult<User>(pageNumber, pageSize, out rowCount, x =>
            x.Email != null && x.OrganizationId == candidate.OrganizationId && x.Id != candidate.Id &&
            (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()) ||
             x.Email.ToLower().Contains(search.ToLower()))).ToList();

        var organizationUserIds = organizationUsers.Select(x => x.Id);

        var trainingCandidates =
            genericRepository.Get<TrainingCandidate>(x =>
                x.TrainingId == training.Id && organizationUserIds.Contains(x.CandidateId) && x.IsActionCompleted &&
                x.IsApproved).ToList();

        var candidateIds = trainingCandidates.Select(x => x.CandidateId).ToList();

        foreach (var candidateIdentifier in candidateIds)
        {
            var candidateUser = genericRepository.GetById<User>(candidateIdentifier) ??
                                throw new NotFoundException(
                                    "The following candidate has not been registered to our system.");

            if (candidateUser.Email != null && !string.IsNullOrEmpty(search) && !(candidateUser.Name.ToLower().Contains(search.ToLower()) ||
                    candidateUser.Email.ToLower().Contains(search.ToLower()))) continue;

            var organization = candidateUser.OrganizationId == null
                ? null
                : genericRepository.GetById<Organization>(candidateUser.OrganizationId) ??
                  throw new NotFoundException("The following organization could not be found.");

            var clientTrainingCandidate = trainingCandidates.FirstOrDefault(x =>
                                              x.CandidateId == candidateIdentifier && x.TrainingId == training.Id)
                                          ?? throw new NotFoundException(
                                              "The respective candidate has not been approved to the following training.");

            candidates.Add(new GetApprovedCandidateDetailsDto()
            {
                TrainingCandidateId = clientTrainingCandidate.Id,
                ApprovedDate = clientTrainingCandidate.ActionDate?.ToFormattedDateTime(),
                Id = candidateUser.Id,
                Name = candidateUser.Name,
                EmailAddress = candidateUser.Email ?? "",
                Gender = candidate.Gender.ToString(),
                PhoneNumber = candidateUser.PhoneNumber ?? "",
                ImageUrl = candidateUser.ImageURL,
                DesignationId = candidate.DesignationId,
                Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null,
                Organization = candidateUser.OrganizationId == null
                    ? null
                    : new GetOrganizationDto()
                    {
                        Id = organization!.Id,
                        Name = organization.Name,
                        Description = organization.Description,
                        IsActive = organization.IsActive,
                        ImageUrl = organization.ImageUrl,
                        Address = organization.Address
                    }
            });
        }

        return candidates;
    }

    public List<GetApprovedCandidateDetailsDto> GetAllOrganizationalCandidatesForTraining(Guid trainingId, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var candidateId = userService.GetUserId;

        var userRole = userService.GetUserRole;

        var candidate = genericRepository.GetById<User>(candidateId) ??
                        throw new NotFoundException("The following candidate has not been registered to our system.");

        if (candidate.OrganizationId == null)
            throw new BadRequestException("Organizational Candidates could not be fetched",
                ["The following user does not belong to a particular client organization."]);

        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
            x.CandidateId == candidate.Id && x.TrainingId == training.Id);

        var candidates = new List<GetApprovedCandidateDetailsDto>();

        if (userRole == Constants.Roles.Candidate)
        {
            if (trainingCandidate == null)
            {
                throw new NotFoundException(
                    "The respective candidate has not been approved to the following training.");
            }
            
            if (candidate.Email != null && (string.IsNullOrEmpty(search) || candidate.Name.ToLower().Contains(search.ToLower()) || candidate.Email.ToLower().Contains(search.ToLower())))
            {
                candidates.Add(new GetApprovedCandidateDetailsDto()
                {
                    TrainingCandidateId = trainingCandidate.Id,
                    ApprovedDate = trainingCandidate.ActionDate?.ToFormattedDateTime(),
                    Id = candidate.Id,
                    Name = candidate.Name,
                    EmailAddress = candidate.Email ?? "",
                    PhoneNumber = candidate.PhoneNumber ?? "",
                    ImageUrl = candidate.ImageURL,
                    Organization = null,
                    Gender = candidate.Gender.ToString(),
                    DesignationId = candidate.DesignationId,
                    Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null
                });
            }
        }

        var organizationUsers = genericRepository.Get<User>(x =>
            x.OrganizationId == candidate.OrganizationId && x.Id != candidate.Id).ToList();

        var organizationUserIds = organizationUsers.Select(x => x.Id);

        var trainingCandidates =
            genericRepository.Get<TrainingCandidate>(x =>
                x.TrainingId == training.Id && organizationUserIds.Contains(x.CandidateId) && x.IsActionCompleted &&
                x.IsApproved).ToList();

        var candidateIds = trainingCandidates.Select(x => x.CandidateId).ToList();

        foreach (var candidateIdentifier in candidateIds)
        {
            var candidateUser = genericRepository.GetById<User>(candidateIdentifier) ??
                                throw new NotFoundException(
                                    "The following candidate has not been registered to our system.");
            
            if (candidateUser.Email != null && !string.IsNullOrEmpty(search) && !(candidateUser.Name.ToLower().Contains(search.ToLower()) || candidateUser.Email.ToLower().Contains(search.ToLower()))) continue;

            var organization = candidateUser.OrganizationId == null
                ? null
                : genericRepository.GetById<Organization>(candidateUser.OrganizationId) ??
                  throw new NotFoundException("The following organization could not be found.");

            var clientTrainingCandidate = trainingCandidates.FirstOrDefault(x =>
                                              x.CandidateId == candidate.Id && x.TrainingId == training.Id)
                                          ?? throw new NotFoundException(
                                              "The respective candidate has not been approved to the following training.");

            candidates.Add(new GetApprovedCandidateDetailsDto()
            {
                TrainingCandidateId = clientTrainingCandidate.Id,
                ApprovedDate = clientTrainingCandidate.ActionDate?.ToFormattedDateTime(),
                Id = candidateUser.Id,
                Name = candidateUser.Name,
                EmailAddress = candidateUser.Email ?? "",
                PhoneNumber = candidateUser.PhoneNumber ?? "",
                ImageUrl = candidateUser.ImageURL,
                DesignationId = candidateUser.DesignationId,
                Gender = candidate.Gender.ToString(),
                Designation = candidateUser.DesignationId != null ? genericRepository.GetById<Designation>(candidateUser.DesignationId)?.Title : null,
                Organization = candidateUser.OrganizationId == null
                    ? null
                    : new GetOrganizationDto()
                    {
                        Id = organization!.Id,
                        Name = organization.Name,
                        Description = organization.Description,
                        IsActive = organization.IsActive,
                        ImageUrl = organization.ImageUrl,
                        Address = organization.Address
                    }
            });
        }

        return candidates;
    }

    public List<GetApprovedCandidateDetailsDto> GetAllAssignedCandidatesForClient(Guid trainingId, int pageNumber, int pageSize, out int rowCount)
    {
        var clientId = userService.GetUserId;

        var client = genericRepository.GetById<User>(clientId)
                     ?? throw new NotFoundException("The following candidate has not been registered to our system.");

        if (client.OrganizationId == null)
            throw new BadRequestException("Organizational Candidates could not be fetched",
                ["The following user does not belong to a particular client organization."]);

        var candidates = new List<GetApprovedCandidateDetailsDto>();

        var organizationUsers = genericRepository.GetPagedResult<User>(pageNumber, pageSize, out rowCount, x =>
            x.OrganizationId == client.OrganizationId && x.Id != client.Id).ToList();

        var organizationUserIds = organizationUsers.Select(x => x.Id);

        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var trainingCandidates =
            genericRepository.Get<TrainingCandidate>(x =>
                x.TrainingId == training.Id && organizationUserIds.Contains(x.CandidateId) && x.IsActionCompleted &&
                x.IsApproved).ToList();

        var candidateIds = trainingCandidates.Select(x => x.CandidateId).ToList();

        foreach (var candidateIdentifier in candidateIds)
        {
            var candidateUser = genericRepository.GetById<User>(candidateIdentifier)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been registered to our system.");

            var organization = candidateUser.OrganizationId == null
                ? null
                : genericRepository.GetById<Organization>(candidateUser.OrganizationId) ??
                  throw new NotFoundException("The following organization could not be found.");

            var trainingCandidate =
                trainingCandidates.FirstOrDefault(x =>
                    x.CandidateId == candidateUser.Id && x.TrainingId == training.Id)
                ?? throw new NotFoundException(
                    "The respective candidate has not been approved to the following training.");

            candidates.Add(new GetApprovedCandidateDetailsDto()
            {
                TrainingCandidateId = trainingCandidate.Id,
                ApprovedDate = trainingCandidate.ActionDate?.ToFormattedDateTime(),
                Id = candidateUser.Id,
                Name = candidateUser.Name,
                EmailAddress = candidateUser.Email ?? "",
                PhoneNumber = candidateUser.PhoneNumber ?? "",
                ImageUrl = candidateUser.ImageURL,
                Gender = candidateUser.Gender.ToString(),
                DesignationId = candidateUser.DesignationId,
                Designation = candidateUser.DesignationId != null ? genericRepository.GetById<Designation>(candidateUser.DesignationId)?.Title : null,
                Organization = candidateUser.OrganizationId == null
                    ? null
                    : new GetOrganizationDto()
                    {
                        Id = organization!.Id,
                        Name = organization.Name,
                        Description = organization.Description,
                        IsActive = organization.IsActive,
                        ImageUrl = organization.ImageUrl,
                        Address = organization.Address
                    }
            });
        }

        return candidates;
    }

    public List<GetApprovedCandidateDetailsDto> GetAllAssignedCandidatesForClient(Guid trainingId)
    {
        var clientId = userService.GetUserId;

        var client = genericRepository.GetById<User>(clientId) ??
                     throw new NotFoundException("The following candidate has not been registered to our system.");

        if (client.OrganizationId == null)
            throw new BadRequestException("Organizational Candidates could not be fetched",
                ["The following user does not belong to a particular client organization."]);

        var candidates = new List<GetApprovedCandidateDetailsDto>();

        var organizationUsers = genericRepository.Get<User>(x =>
            x.OrganizationId == client.OrganizationId && x.Id != client.Id).ToList();

        var organizationUserIds = organizationUsers.Select(x => x.Id);

        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var trainingCandidates =
            genericRepository.Get<TrainingCandidate>(x =>
                x.TrainingId == training.Id && organizationUserIds.Contains(x.CandidateId) && x.IsActionCompleted &&
                x.IsApproved).ToList();

        var candidateIds = trainingCandidates.Select(x => x.CandidateId).ToList();

        foreach (var candidateIdentifier in candidateIds)
        {
            var candidateUser = genericRepository.GetById<User>(candidateIdentifier)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been registered to our system.");

            var organization = candidateUser.OrganizationId == null
                ? null
                : genericRepository.GetById<Organization>(candidateUser.OrganizationId) ??
                  throw new NotFoundException("The following organization could not be found.");

            var trainingCandidate =
                trainingCandidates.FirstOrDefault(x =>
                    x.CandidateId == candidateUser.Id && x.TrainingId == training.Id)
                ?? throw new NotFoundException(
                    "The respective candidate has not been approved to the following training.");

            candidates.Add(new GetApprovedCandidateDetailsDto()
            {
                TrainingCandidateId = trainingCandidate.Id,
                ApprovedDate = trainingCandidate.ActionDate?.ToFormattedDateTime(),
                Id = candidateUser.Id,
                Name = candidateUser.Name,
                EmailAddress = candidateUser.Email ?? "",
                PhoneNumber = candidateUser.PhoneNumber ?? "",
                ImageUrl = candidateUser.ImageURL,
                Gender = candidateUser.Gender.ToString(),
                Organization = candidateUser.OrganizationId == null
                    ? null
                    : new GetOrganizationDto()
                    {
                        Id = organization!.Id,
                        Name = organization.Name,
                        Description = organization.Description,
                        IsActive = organization.IsActive,
                        ImageUrl = organization.ImageUrl,
                        Address = organization.Address
                    },
                DesignationId = candidateUser.DesignationId,
                Designation = candidateUser.DesignationId != null ? genericRepository.GetById<Designation>(candidateUser.DesignationId)?.Title : null,
            });
        }

        return candidates;
    }

    public List<GetAllTrainingsForCandidate> GetAllTrainingsForCandidate(int requestAction, int pageNumber,
        int pageSize, out int rowCount, string? search = null)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x =>
            x.CandidateId == candidateId).ToList();

        var identifiers = requestAction switch
        {
            Constants.RequestAction.Pending =>
                trainingCandidates.Where(z => !z.IsActionCompleted).Select(z => z.TrainingId).ToList(),
            Constants.RequestAction.Rejected =>
                trainingCandidates.Where(z => z is { IsApproved: false, IsActionCompleted: true })
                    .Select(z => z.TrainingId).ToList(),
            Constants.RequestAction.Available =>
                trainingCandidates.Select(z => z.TrainingId).ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(requestAction), requestAction, null)
        };

        var trainings = requestAction switch
        {
            Constants.RequestAction.Pending =>
                genericRepository.GetPagedResult<Training>(pageNumber, pageSize, out rowCount, tc =>
                        identifiers.Contains(tc.Id) && tc.IsActive && (string.IsNullOrEmpty(search) ||
                                                                       tc.Title.ToLower().Contains(search.ToLower())))
                    .ToList(),
            Constants.RequestAction.Rejected =>
                genericRepository.GetPagedResult<Training>(pageNumber, pageSize, out rowCount, tc =>
                        identifiers.Contains(tc.Id) && tc.IsActive && (string.IsNullOrEmpty(search) ||
                                                                       tc.Title.ToLower().Contains(search.ToLower())))
                    .ToList(),
            Constants.RequestAction.Available =>
                genericRepository.GetPagedResult<Training>(pageNumber, pageSize, out rowCount, tc =>
                    !identifiers.Contains(tc.Id) && tc.IsActive && tc.EndDate >= DateOnly.FromDateTime(DateTime.Now) &&
                    (string.IsNullOrEmpty(search) || tc.Title.ToLower().Contains(search.ToLower()))).ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(requestAction), requestAction, null)
        };

        var result = new List<GetAllTrainingsForCandidate>();

        foreach (var training in trainings)
        {
            var trainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)
                                 ?? throw new NotFoundException("The following training format could not be found.");

            var action = requestAction switch
            {
                Constants.RequestAction.Pending => Constants.RequestAction.PendingAction,
                Constants.RequestAction.Rejected => Constants.RequestAction.RejectedAction,
                Constants.RequestAction.Available => Constants.RequestAction.AvailableAction,
                _ => throw new ArgumentOutOfRangeException(nameof(requestAction), requestAction, null)
            };

            if (requestAction == Constants.RequestAction.Available)
            {
                result.Add(new GetAllTrainingsForCandidate
                {
                    Id = training.Id,
                    Title = training.Title,
                    Description = training.Description,
                    IsActive = training.IsActive,
                    StartDate = training.StartDate.ToFormattedDate(),
                    EndDate = training.EndDate.ToFormattedDate(),
                    LocationDetails = training.LocationDetails,
                    Latitude = training.Latitude ?? 0m,
                    Longitude = training.Longitude ?? 0m,
                    ImageUrl = training.ImageUrl,
                    TrainingFormatId = trainingFormat.Id,
                    TrainingFormat = trainingFormat.Name,
                    Action = action,
                    AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
                });
            }
            else
            {
                var trainingCandidateModel =
                    trainingCandidates.FirstOrDefault(x => x.CandidateId == candidate.Id && x.TrainingId == training.Id)
                    ?? throw new NotFoundException(
                        "The following user has not requested or been assigned to any trainings based on the request action.");

                result.Add(new GetAllTrainingsForCandidate
                {
                    TrainingCandidateId = trainingCandidateModel.Id,
                    Id = training.Id,
                    Title = training.Title,
                    Description = training.Description,
                    IsActive = training.IsActive,
                    StartDate = training.StartDate.ToFormattedDate(),
                    EndDate = training.EndDate.ToFormattedDate(),
                    LocationDetails = training.LocationDetails,
                    Latitude = training.Latitude ?? 0m,
                    Longitude = training.Longitude ?? 0m,
                    ImageUrl = training.ImageUrl,
                    TrainingFormatId = trainingFormat.Id,
                    TrainingFormat = trainingFormat.Name,
                    Action = action,
                    ActionDate = trainingCandidateModel.ActionDate?.ToFormattedDateTime(),
                    Remarks = trainingCandidateModel.Remarks,
                    AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
                });
            }
        }

        return result;
    }

    public List<GetAllTrainingsForCandidate> GetAllTrainingsForCandidate(int requestAction, string? search = null)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x =>
            x.CandidateId == candidateId).ToList();

        var identifiers = requestAction switch
        {
            Constants.RequestAction.Pending =>
                trainingCandidates.Where(z => !z.IsActionCompleted).Select(z => z.TrainingId).ToList(),
            Constants.RequestAction.Rejected =>
                trainingCandidates.Where(z => z is { IsApproved: false, IsActionCompleted: true })
                    .Select(z => z.TrainingId).ToList(),
            Constants.RequestAction.Available =>
                trainingCandidates.Select(z => z.TrainingId).ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(requestAction), requestAction, null)
        };

        var trainings = requestAction switch
        {
            Constants.RequestAction.Pending =>
                genericRepository.Get<Training>(tc =>
                        identifiers.Contains(tc.Id) && tc.IsActive && (string.IsNullOrEmpty(search) ||
                                                                       tc.Title.ToLower().Contains(search.ToLower())))
                    .ToList(),
            Constants.RequestAction.Rejected =>
                genericRepository.Get<Training>(tc =>
                        identifiers.Contains(tc.Id) && tc.IsActive && (string.IsNullOrEmpty(search) ||
                                                                       tc.Title.ToLower().Contains(search.ToLower())))
                    .ToList(),
            Constants.RequestAction.Available =>
                genericRepository.Get<Training>(tc =>
                    !identifiers.Contains(tc.Id) && tc.IsActive && tc.EndDate >= DateOnly.FromDateTime(DateTime.Now) &&
                    (string.IsNullOrEmpty(search) || tc.Title.ToLower().Contains(search.ToLower()))).ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(requestAction), requestAction, null)
        };

        var result = new List<GetAllTrainingsForCandidate>();

        foreach (var training in trainings)
        {
            var trainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)
                                 ?? throw new NotFoundException("The following training format could not be found.");

            var action = requestAction switch
            {
                Constants.RequestAction.Pending => Constants.RequestAction.PendingAction,
                Constants.RequestAction.Rejected => Constants.RequestAction.RejectedAction,
                Constants.RequestAction.Available => Constants.RequestAction.AvailableAction,
                _ => throw new ArgumentOutOfRangeException(nameof(requestAction), requestAction, null)
            };

            if (requestAction == Constants.RequestAction.Available)
            {
                result.Add(new GetAllTrainingsForCandidate
                {
                    Id = training.Id,
                    Title = training.Title,
                    Description = training.Description,
                    IsActive = training.IsActive,
                    StartDate = training.StartDate.ToFormattedDate(),
                    EndDate = training.EndDate.ToFormattedDate(),
                    LocationDetails = training.LocationDetails,
                    Latitude = training.Latitude ?? 0m,
                    Longitude = training.Longitude ?? 0m,
                    ImageUrl = training.ImageUrl,
                    TrainingFormatId = trainingFormat.Id,
                    TrainingFormat = trainingFormat.Name,
                    Action = action,
                    AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
                });
            }
            else
            {
                var trainingCandidateModel =
                    trainingCandidates.FirstOrDefault(x => x.CandidateId == candidate.Id && x.TrainingId == training.Id)
                    ?? throw new NotFoundException(
                        "The following user has not requested or been assigned to any trainings based on the request action.");

                result.Add(new GetAllTrainingsForCandidate
                {
                    TrainingCandidateId = trainingCandidateModel.Id,
                    Id = training.Id,
                    Title = training.Title,
                    Description = training.Description,
                    IsActive = training.IsActive,
                    StartDate = training.StartDate.ToFormattedDate(),
                    EndDate = training.EndDate.ToFormattedDate(),
                    LocationDetails = training.LocationDetails,
                    Latitude = training.Latitude ?? 0m,
                    Longitude = training.Longitude ?? 0m,
                    ImageUrl = training.ImageUrl,
                    TrainingFormatId = trainingFormat.Id,
                    TrainingFormat = trainingFormat.Name,
                    Action = action,
                    ActionDate = trainingCandidateModel.ActionDate?.ToFormattedDateTime(),
                    Remarks = trainingCandidateModel.Remarks,
                    AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
                });
            }
        }

        return result;
    }

    public AvailableTrainingCountDto GetAllAvailableTrainingCountsForCandidate()
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x =>
            x.CandidateId == candidateId && x.IsActive).ToList();

        return new AvailableTrainingCountDto
        {
            PendingCount =
                genericRepository.GetCount<TrainingCandidate>(
                    x => x.CandidateId == candidate.Id && !x.IsActionCompleted && x.IsActive),
            RejectedCount = genericRepository.GetCount<TrainingCandidate>(x =>
                x.CandidateId == candidate.Id && x.IsActionCompleted && !x.IsApproved && x.IsActive),
            AvailableCount = genericRepository.GetCount<Training>(x =>
                !trainingCandidates.Select(z => z.TrainingId).Contains(x.Id) && x.IsActive &&
                x.EndDate >= DateOnly.FromDateTime(DateTime.Now))
        };
    }

    public List<GetAllTrainingsForCandidate> GetAllAssignedTrainingsForCandidate(int statusAction, int pageNumber,
        int pageSize, out int rowCount, string? search = null)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException("The following candidate has not been registered to our system.");

        var trainingCondition = GetTrainingDetailsCondition(statusAction, search);

        var query = from tc in genericRepository.Get<TrainingCandidate>()
                    join t in genericRepository.Get<Training>().Where(trainingCondition)
                        on tc.TrainingId equals t.Id
                    where tc.CandidateId == candidate.Id && tc.IsApproved
                    select new
                    {
                        Training = t,
                        TrainingCandidate = tc
                    };

        rowCount = query.Count();

        var pagedItems = query
            .OrderBy(x => x.Training.StartDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var trainingFormatIds = pagedItems.Select(x => x.Training.TrainingFormatId).Distinct().ToList();
        var formats = genericRepository.Get<TrainingFormat>(x => trainingFormatIds.Contains(x.Id)).ToList();

        var result = pagedItems.Select(item =>
        {
            var format = formats.FirstOrDefault(f => f.Id == item.Training.TrainingFormatId)
                ?? throw new NotFoundException("Training format not found.");

            return new GetAllTrainingsForCandidate
            {
                TrainingCandidateId = item.TrainingCandidate.Id,
                Id = item.Training.Id,
                Title = item.Training.Title,
                Description = item.Training.Description,
                IsActive = item.Training.IsActive,
                StartDate = item.Training.StartDate.ToFormattedDate(),
                EndDate = item.Training.EndDate.ToFormattedDate(),
                LocationDetails = item.Training.LocationDetails,
                Latitude = item.Training.Latitude ?? 0m,
                Longitude = item.Training.Longitude ?? 0m,
                ImageUrl = item.Training.ImageUrl,
                TrainingFormatId = format.Id,
                TrainingFormat = format.Name,
                Action = statusAction switch
                {
                    Constants.StatusAction.Available => Constants.StatusAction.AvailableAction,
                    Constants.StatusAction.Expired => Constants.StatusAction.ExpiredAction,
                    _ => Constants.StatusAction.AllAction
                },
                ActionDate = item.TrainingCandidate.ActionDate?.ToFormattedDateTime(),
                Remarks = item.TrainingCandidate.Remarks,
                AssignedTrainers = GetAssignedTrainingsTrainers(item.Training.Id)
            };
        }).ToList();

        return result;
    }

    public List<GetAllTrainingsForCandidate> GetAllAssignedTrainingsForCandidate(int statusAction,
        string? search = null)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException("The following candidate has not been registered to our system.");

        var trainingCondition = GetTrainingDetailsCondition(statusAction, search);

        var query = from tc in genericRepository.Get<TrainingCandidate>()
                    join t in genericRepository.Get<Training>().Where(trainingCondition)
                        on tc.TrainingId equals t.Id
                    where tc.CandidateId == candidate.Id && tc.IsApproved
                    select new
                    {
                        Training = t,
                        TrainingCandidate = tc
                    };

        var pagedItems = query
            .OrderBy(x => x.Training.StartDate)
            .ToList();

        var trainingFormatIds = pagedItems.Select(x => x.Training.TrainingFormatId).Distinct().ToList();
        var formats = genericRepository.Get<TrainingFormat>(x => trainingFormatIds.Contains(x.Id)).ToList();

        var result = pagedItems.Select(item =>
        {
            var format = formats.FirstOrDefault(f => f.Id == item.Training.TrainingFormatId)
                ?? throw new NotFoundException("Training format not found.");

            return new GetAllTrainingsForCandidate
            {
                TrainingCandidateId = item.TrainingCandidate.Id,
                Id = item.Training.Id,
                Title = item.Training.Title,
                Description = item.Training.Description,
                IsActive = item.Training.IsActive,
                StartDate = item.Training.StartDate.ToFormattedDate(),
                EndDate = item.Training.EndDate.ToFormattedDate(),
                LocationDetails = item.Training.LocationDetails,
                Latitude = item.Training.Latitude ?? 0m,
                Longitude = item.Training.Longitude ?? 0m,
                ImageUrl = item.Training.ImageUrl,
                TrainingFormatId = format.Id,
                TrainingFormat = format.Name,
                Action = statusAction switch
                {
                    Constants.StatusAction.Available => Constants.StatusAction.AvailableAction,
                    Constants.StatusAction.Expired => Constants.StatusAction.ExpiredAction,
                    _ => Constants.StatusAction.AllAction
                },
                ActionDate = item.TrainingCandidate.ActionDate?.ToFormattedDateTime(),
                Remarks = item.TrainingCandidate.Remarks,
                AssignedTrainers = GetAssignedTrainingsTrainers(item.Training.Id)
            };
        }).ToList();

        return result;
    }

    public AssignedTrainingCountDto GetAllAssignedTrainingCountsForCandidate()
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var approvedTrainingCandidates = genericRepository.Get<TrainingCandidate>(x =>
            x.CandidateId == candidate.Id && x.IsApproved).ToList();

        return new AssignedTrainingCountDto()
        {
            AllCount = genericRepository.GetCount<Training>(x =>
                approvedTrainingCandidates.Select(z => z.TrainingId).Contains(x.Id) && x.IsActive),
            AvailableCount = genericRepository.GetCount<Training>(x =>
                approvedTrainingCandidates.Select(z =>
                    z.TrainingId).Contains(x.Id) && x.IsActive && x.EndDate >= DateOnly.FromDateTime(DateTime.Now)),
            ExpiredCount = genericRepository.GetCount<Training>(x =>
                approvedTrainingCandidates.Select(z =>
                    z.TrainingId).Contains(x.Id) && x.IsActive && x.EndDate < DateOnly.FromDateTime(DateTime.Now)),
        };
    }

    public TrainingDetailsCountDto GetTrainingDetailsCountForCandidate(Guid trainingId)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException("The following candidate could not be found.");
        
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                                    x.TrainingId == training.Id && x.CandidateId == candidate.Id && x.IsApproved)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been approved to the respective training.");
        
        var classCount = genericRepository.GetCount<Class>(x => x.TrainingId == training.Id);

        var resourceCount = genericRepository.GetCount<TrainingResources>(x => x.TrainingId == training.Id && x.IsActive);
        
        var candidateCount = genericRepository.GetCount<TrainingCandidate>(x =>
            x.TrainingId == training.Id && x.IsApproved);

        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();
        
        var classTrainers = genericRepository.Get<ClassTrainer>(x => classes.Select(z => z.Id).Contains(x.ClassId)).ToList();
        
        var trainerCount = classTrainers.Select(x => x.TrainerId).Distinct().Count();
        
        var trainingInspections = genericRepository.Get<TrainingInspection>(x => x.TrainingId == training.Id).ToList();
        
        var questionnaireCount =
            genericRepository.GetCount<Questionnaire>(x =>
                x.TrainingInspectionId != null && trainingInspections.Select(z => z.Id).Contains(x.TrainingInspectionId.Value));

        var subordinateCount =
            genericRepository.GetCount<Subordinate>(x => x.TrainingCandidateId == trainingCandidate.Id);

        var certificationCount =
            genericRepository.GetCount<Certificate>(x => x.TrainingCandidateId == trainingCandidate.Id);
        
        return new TrainingDetailsCountDto()
        {
            ClassDetailsCount = classCount,
            ResourceDetailsCount = resourceCount,
            CandidateDetailsCount = candidateCount,
            InspectionCount = questionnaireCount,
            TrainerDetailsCount = trainerCount,
            SubordinateCount = subordinateCount,
            CertificationCount = certificationCount
        };
    }
    
    public List<GetAllTrainingsForClient> GetAllTrainingsForClient(int requestAction, int pageNumber, int pageSize,
        out int rowCount, string? search = null)
    {
        var trainings = GetAllActiveTrainings(pageNumber, pageSize, out rowCount, search);

        var trainingCandidates = GetAllClientTrainingCandidates();

        var result = new List<GetAllTrainingsForClient>();

        foreach (var training in trainings)
        {
            var candidateCount = trainingCandidates.Count(tc =>
                tc.TrainingId == training.Id && tc is { IsActionCompleted: true, IsApproved: true });

            if (requestAction == Constants.RequestAction.Accepted)
            {
                if (candidateCount == 0) continue;
            }

            result.Add(new GetAllTrainingsForClient
            {
                Id = training.Id,
                Title = training.Title,
                Description = training.Description,
                IsActive = training.IsActive,
                EndDate = training.EndDate,
                StartDate = training.StartDate,
                LocationDetails = training.LocationDetails,
                Latitude = training.Latitude,
                Longitude = training.Longitude,
                ImageUrl = training.ImageUrl,
                TrainingFormatId = training.TrainingFormatId,
                TrainingFormat = training.TrainingFormat,
                CandidateCount = candidateCount,
                AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
            });
        }

        return result;
    }

    public List<GetAllTrainingsForClient> GetAllTrainingsForClient(int requestAction, string? search = null)
    {
        var trainings = GetAllActiveTrainings(search);

        var trainingCandidates = GetAllClientTrainingCandidates();

        var result = new List<GetAllTrainingsForClient>();

        foreach (var training in trainings)
        {
            var candidateCount = trainingCandidates.Count(tc =>
                tc.TrainingId == training.Id && tc is { IsActionCompleted: true, IsApproved: true });

            if (requestAction == Constants.RequestAction.Accepted)
            {
                if (candidateCount == 0) continue;
            }

            result.Add(new GetAllTrainingsForClient
            {
                Id = training.Id,
                Title = training.Title,
                Description = training.Description,
                IsActive = training.IsActive,
                EndDate = training.EndDate,
                StartDate = training.StartDate,
                LocationDetails = training.LocationDetails,
                Latitude = training.Latitude,
                Longitude = training.Longitude,
                ImageUrl = training.ImageUrl,
                TrainingFormatId = training.TrainingFormatId,
                TrainingFormat = training.TrainingFormat,
                CandidateCount = candidateCount,
                AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
            });
        }

        return result;
    }

    public List<GetCandidateDetailsDto> GetAllUnassignedClientCandidatesForTraining(Guid trainingId)
    {
        var userId = userService.GetUserId;

        var userModel = genericRepository.GetById<User>(userId)
                   ?? throw new NotFoundException("The following user could not be found.");

        if (userModel.OrganizationId == null)
        {
            throw new NotFoundException("The following user is not assigned to any client organization could not be found.");
        }
        
        var organizationModel = genericRepository.GetById<Organization>(userModel.OrganizationId)
                                ?? throw new NotFoundException("The following organization could not be found.");
        
        var training = genericRepository.GetById<Training>(trainingId) 
                       ?? throw new NotFoundException("The following training could not be found.");

        var requestedCandidateIds = genericRepository.Get<TrainingCandidate>(x => 
                x.TrainingId == training.Id).Select(x => 
                    x.CandidateId).ToList();

        var candidateRole = genericRepository.GetFirstOrDefault<Role>(x => x.Name == Constants.Roles.Candidate)
            ?? throw new NotFoundException("The following role could not be found.");

        var candidateUserRoles = genericRepository.Get<UserRoles>(x => 
            x.RoleId == candidateRole.Id).ToList();

        var unassignedCandidates = genericRepository.Get<User>(x => 
                candidateUserRoles.Select(z => 
                    z.UserId).Contains(x.Id) && !requestedCandidateIds.Contains(x.Id) && x.IsActive && x.OrganizationId == organizationModel.Id).ToList();

        return (from candidate in unassignedCandidates
                let organization = candidate.OrganizationId == null
                ? null
                : genericRepository.GetById<Organization>(candidate.OrganizationId) 
                  ?? throw new NotFoundException("The organization could not be found for this candidate.")
            select new GetCandidateDetailsDto()
            {
                Id = candidate.Id,
                Name = candidate.Name,
                EmailAddress = candidate.Email ?? "",
                PhoneNumber = candidate.PhoneNumber ?? "",
                ImageUrl = candidate.ImageURL,
                Gender = candidate.Gender.ToString(),
                DesignationId = candidate.DesignationId,
                Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null,
                Organization = candidate.OrganizationId == null
                    ? null
                    : new GetOrganizationDto()
                    {
                        Id = organization!.Id,
                        Name = organization.Name,
                        Description = organization.Description,
                        IsActive = organization.IsActive,
                        ImageUrl = organization.ImageUrl ??  "",
                        Address = organization.Address
                    }
            }).ToList();
    }

    public AvailableTrainingCountDto GetAllAvailableTrainingCountsForClient()
    {
        var clientId = userService.GetUserId;

        var client = genericRepository.GetById<User>(clientId)
                     ?? throw new NotFoundException(
                         "The following candidate has not been registered to our system.");

        var clientCandidates = genericRepository.Get<User>(x =>
            x.OrganizationId == client.OrganizationId).ToList();

        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x =>
            clientCandidates.Select(z => z.Id).Contains(x.CandidateId)).ToList();

        return new AvailableTrainingCountDto
        {
            PendingCount =
                genericRepository.GetCount<TrainingCandidate>(
                    x => clientCandidates.Select(z => z.Id).Contains(x.CandidateId) && !x.IsActionCompleted),
            RejectedCount = genericRepository.GetCount<TrainingCandidate>(x =>
                clientCandidates.Select(z => z.Id).Contains(x.CandidateId) && x.IsActionCompleted && !x.IsApproved),
            AvailableCount = genericRepository.GetCount<Training>(x =>
                !trainingCandidates.Select(z => z.TrainingId).Contains(x.Id))
        };
    }

    public List<GetAllTrainingsForClient> GetAllAssignedTrainingsForClient(int statusAction, int pageNumber, 
        int pageSize, out int rowCount, string? search = null)
    {
        var clientOrganizationId = userService.GetUserId;

        var clientOrganization = genericRepository.GetById<User>(clientOrganizationId)
            ?? throw new NotFoundException("Client organization admin not found.");

        if (clientOrganization.OrganizationId == null)
            throw new NotFoundException("Client organization does not belong to any organization.");

        var organization = genericRepository.GetById<Organization>(clientOrganization.OrganizationId.Value)
            ?? throw new NotFoundException("Organization not found.");

        var candidateIds = genericRepository.Get<User>(x =>
            x.OrganizationId == organization.Id && x.Id != clientOrganization.Id)
            .Select(x => x.Id)
            .ToList();

        var trainingCandidateQuery = from tc in genericRepository.Get<TrainingCandidate>()
                                     where candidateIds.Contains(tc.CandidateId) && tc.IsApproved
                                     select tc;

        var trainingFilter = GetTrainingDetailsCondition(statusAction, search);

        var filteredQuery = from tc in trainingCandidateQuery
                            join t in genericRepository.Get<Training>().Where(trainingFilter)
                                on tc.TrainingId equals t.Id
                            select new
                            {
                                tc.TrainingId,
                                tc.IsActionCompleted
                            };

        var groupedQuery = filteredQuery
            .GroupBy(x => x.TrainingId)
            .Select(g => new
            {
                TrainingId = g.Key,
                CandidateCount = g.Count(x => x.IsActionCompleted)
            });

        rowCount = groupedQuery.Count();

        var pagedTrainingIds = groupedQuery
            .OrderBy(x => x.TrainingId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var trainingIds = pagedTrainingIds.Select(x => x.TrainingId).ToList();

        var trainings = genericRepository.Get<Training>(x => trainingIds.Contains(x.Id)).ToList();
        var formats = genericRepository.Get<TrainingFormat>().ToList();

        var result = new List<GetAllTrainingsForClient>();

        foreach (var training in trainings)
        {
            var format = formats.FirstOrDefault(f => f.Id == training.TrainingFormatId)
                ?? throw new NotFoundException("Training format not found.");

            var candidateCount = pagedTrainingIds
                .First(x => x.TrainingId == training.Id).CandidateCount;

            result.Add(new GetAllTrainingsForClient
            {
                Id = training.Id,
                Title = training.Title,
                Description = training.Description,
                IsActive = training.IsActive,
                EndDate = training.EndDate.ToFormattedDate(),
                StartDate = training.StartDate.ToFormattedDate(),
                LocationDetails = training.LocationDetails,
                Latitude = training.Latitude ?? 0m,
                Longitude = training.Longitude ?? 0m,
                ImageUrl = training.ImageUrl,
                TrainingFormatId = training.TrainingFormatId,
                TrainingFormat = format.Name,
                CandidateCount = candidateCount,
                AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
            });
        }

        return result;
    }

    public List<GetAllTrainingsForClient> GetAllAssignedTrainingsForClient(int statusAction, string? search = null)
    {
        var clientOrganizationId = userService.GetUserId;

        var clientOrganization = genericRepository.GetById<User>(clientOrganizationId)
            ?? throw new NotFoundException("Client organization admin not found.");

        if (clientOrganization.OrganizationId == null)
            throw new NotFoundException("Client organization does not belong to any organization.");

        var organization = genericRepository.GetById<Organization>(clientOrganization.OrganizationId.Value)
            ?? throw new NotFoundException("Organization not found.");

        var candidateIds = genericRepository.Get<User>(x =>
            x.OrganizationId == organization.Id && x.Id != clientOrganization.Id)
            .Select(x => x.Id)
            .ToList();

        var trainingCandidateQuery = from tc in genericRepository.Get<TrainingCandidate>()
                                     where candidateIds.Contains(tc.CandidateId) && tc.IsApproved
                                     select tc;

        var trainingFilter = GetTrainingDetailsCondition(statusAction, search);

        var filteredQuery = from tc in trainingCandidateQuery
                            join t in genericRepository.Get<Training>().Where(trainingFilter)
                                on tc.TrainingId equals t.Id
                            select new
                            {
                                tc.TrainingId,
                                tc.IsActionCompleted
                            };

        var groupedQuery = filteredQuery
            .GroupBy(x => x.TrainingId)
            .Select(g => new
            {
                TrainingId = g.Key,
                CandidateCount = g.Count(x => x.IsActionCompleted)
            });

        var pagedTrainingIds = groupedQuery
            .OrderBy(x => x.TrainingId)
            .ToList();

        var trainingIds = pagedTrainingIds.Select(x => x.TrainingId).ToList();

        var trainings = genericRepository.Get<Training>(x => trainingIds.Contains(x.Id)).ToList();
        var formats = genericRepository.Get<TrainingFormat>().ToList();

        var result = new List<GetAllTrainingsForClient>();

        foreach (var training in trainings)
        {
            var format = formats.FirstOrDefault(f => f.Id == training.TrainingFormatId)
                ?? throw new NotFoundException("Training format not found.");

            var candidateCount = pagedTrainingIds
                .First(x => x.TrainingId == training.Id).CandidateCount;

            result.Add(new GetAllTrainingsForClient
            {
                Id = training.Id,
                Title = training.Title,
                Description = training.Description,
                IsActive = training.IsActive,
                EndDate = training.EndDate.ToFormattedDate(),
                StartDate = training.StartDate.ToFormattedDate(),
                LocationDetails = training.LocationDetails,
                Latitude = training.Latitude ?? 0m,
                Longitude = training.Longitude ?? 0m,
                ImageUrl = training.ImageUrl,
                TrainingFormatId = training.TrainingFormatId,
                TrainingFormat = format.Name,
                CandidateCount = candidateCount,
                AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
            });
        }

        return result;
    }

    public AssignedTrainingCountDto GetAllAssignedTrainingCountsForClient()
    {
        var clientId = userService.GetUserId;

        var client = genericRepository.GetById<User>(clientId)
                     ?? throw new NotFoundException(
                         "The following candidate has not been registered to our system.");

        var clientCandidates = genericRepository.Get<User>(x =>
            x.OrganizationId == client.OrganizationId).ToList();

        var approvedTrainingCandidates = genericRepository.Get<TrainingCandidate>(x =>
            clientCandidates.Select(z => z.Id).Contains(x.CandidateId) && x.IsApproved).ToList();

        return new AssignedTrainingCountDto()
        {
            AllCount = genericRepository.GetCount<Training>(x =>
                approvedTrainingCandidates.Select(z =>
                    z.TrainingId).Contains(x.Id) && x.IsActive),
            AvailableCount = genericRepository.GetCount<Training>(x =>
                approvedTrainingCandidates.Select(z =>
                    z.TrainingId).Contains(x.Id) && x.IsActive && x.EndDate >= DateOnly.FromDateTime(DateTime.Now)),
            ExpiredCount = genericRepository.GetCount<Training>(x =>
                approvedTrainingCandidates.Select(z =>
                    z.TrainingId).Contains(x.Id) && x.IsActive && x.EndDate <= DateOnly.FromDateTime(DateTime.Now)),
        };
    }

    public TrainingDetailsCountDto GetTrainingDetailsCountForClient(Guid trainingId)
    {
        var userId = userService.GetUserId;

        var clientUser = genericRepository.GetById<User>(userId)
                         ?? throw new NotFoundException("The following user has not been registered to our system.");
        
        if (clientUser.OrganizationId == null) 
            throw new NotFoundException("The following user does not belong to a particular organization.");
        
        var organization = genericRepository.GetById<Organization>(clientUser.OrganizationId)
            ?? throw new NotFoundException("The following organization could not be found.");
        
        var clientNominatedCandidates = genericRepository.Get<User>(x => 
            x.OrganizationId == organization.Id).ToList();
        
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");
        
        var classCount = genericRepository.GetCount<Class>(x => x.TrainingId == training.Id);

        var resourceCount = genericRepository.GetCount<TrainingResources>(x => x.TrainingId == training.Id && x.IsActive);
        
        var candidateCount = genericRepository.GetCount<TrainingCandidate>(x =>
            x.TrainingId == training.Id && clientNominatedCandidates.Select(z => z.Id).Contains(x.CandidateId) && x.IsApproved);

        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();
        
        var classTrainers = genericRepository.Get<ClassTrainer>(x => classes.Select(z => z.Id).Contains(x.ClassId)).ToList();
        
        var trainerCount = classTrainers.Select(x => x.TrainerId).Distinct().Count();
        
        var trainingInspections = genericRepository.Get<TrainingInspection>(x => x.TrainingId == training.Id).ToList();
        
        var questionnaireCount =
            genericRepository.GetCount<Questionnaire>(x =>
                x.TrainingInspectionId != null && trainingInspections.Select(z => z.Id).Contains(x.TrainingInspectionId.Value));

        return new TrainingDetailsCountDto()
        {
            ClassDetailsCount = classCount,
            ResourceDetailsCount = resourceCount,
            CandidateDetailsCount = candidateCount,
            InspectionCount = questionnaireCount,
            TrainerDetailsCount = trainerCount,
        };
    }
    
    #region Private Methods

    private List<GetTrainingDto> GetAllActiveTrainings(int pageNumber, int pageSize, out int rowCount,
        string? search = null)
    {
        var trainings = genericRepository.GetPagedResult<Training>(pageNumber,
            pageSize,
            out rowCount,
            x => x.IsActive && x.EndDate >= DateOnly.FromDateTime(DateTime.Now) &&
                 (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower()))).ToList();

        return trainings.Select(training => new GetTrainingDto()
        {
            Id = training.Id,
            Description = training.Description,
            Title = training.Title,
            LocationDetails = training.LocationDetails,
            Latitude = training.Latitude ?? 0m,
            Longitude = training.Longitude ?? 0m,
            StartDate = training.StartDate.ToFormattedDate(),
            EndDate = training.EndDate.ToFormattedDate(),
            TrainingFormatId = training.TrainingFormatId,
            TrainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)!.Name,
            ImageUrl = training.ImageUrl,
            IsActive = training.IsActive,
            AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
        }).ToList();
    }

    private List<GetTrainingDto> GetAllActiveTrainings(string? search = null)
    {
        var trainings = genericRepository.Get<Training>(x =>
            x.IsActive && (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower()))).ToList();

        return trainings.Select(training => new GetTrainingDto()
        {
            Id = training.Id,
            Description = training.Description,
            Title = training.Title,
            LocationDetails = training.LocationDetails,
            Latitude = training.Latitude ?? 0m,
            Longitude = training.Longitude ?? 0m,
            StartDate = training.StartDate.ToFormattedDate(),
            EndDate = training.EndDate.ToFormattedDate(),
            TrainingFormatId = training.TrainingFormatId,
            TrainingFormat = genericRepository.GetById<TrainingFormat>(training.TrainingFormatId)?.Name ?? "",
            ImageUrl = training.ImageUrl,
            IsActive = training.IsActive,
            AssignedTrainers = GetAssignedTrainingsTrainers(training.Id)
        }).ToList();
    }

    private List<TrainingCandidate> GetAllClientTrainingCandidates(Guid? trainingId = null)
    {
        var clientUserId = userService.GetUserId;

        var clientUser = genericRepository.GetById<User>(clientUserId)
                         ?? throw new NotFoundException(
                             "The following candidate has not been registered to our system");

        var organizationId = clientUser.OrganizationId;

        if (organizationId == null || organizationId == Guid.Empty)
            throw new NotFoundException("The following organization has not been registered to our system");

        var organization = genericRepository.GetById<Organization>(organizationId)
                           ?? throw new NotFoundException(
                               "The following organization has not been registered to our system");

        var candidateRole = genericRepository.GetFirstOrDefault<Role>(r => r.Name == Constants.Roles.Candidate)!;

        var userRoles = genericRepository.Get<UserRoles>(ur => ur.RoleId == candidateRole.Id).ToList();

        var candidateUserIds = userRoles.Select(u => u.UserId).ToList();

        var organizationCandidate = genericRepository.Get<User>(u =>
            u.OrganizationId == organization.Id && candidateUserIds.Contains(u.Id)).ToList();

        var organizationCandidateIds = organizationCandidate.Select(o => o.Id).ToList();

        List<TrainingCandidate> trainingCandidates = new();

        if (trainingId != null)
        {
            var training = genericRepository.GetById<Training>(trainingId)
                           ?? throw new NotFoundException("The following training could not be found.");

            if (training.IsActive)
            {
                trainingCandidates = genericRepository.Get<TrainingCandidate>(tc =>
                    tc.TrainingId == training.Id && organizationCandidateIds.Contains(tc.CandidateId)).ToList();
            }
        }
        else
        {
            var allActiveTraining = GetAllActiveTrainings();

            trainingCandidates = genericRepository.Get<TrainingCandidate>(tc =>
                allActiveTraining.Select(t => t.Id).Contains(tc.TrainingId) &&
                organizationCandidateIds.Contains(tc.CandidateId)).ToList();
        }

        return trainingCandidates;
    }

    private static Expression<Func<Training, bool>> GetTrainingDetailsCondition(int statusAction, string? search = null)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        Expression<Func<Training, bool>> condition =
            statusAction switch
            {
                Constants.StatusAction.Available => x =>
                    x.EndDate >= currentDate &&
                    (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())) && x.IsActive,
                Constants.StatusAction.Expired => x =>
                    x.StartDate < currentDate && x.EndDate < currentDate && x.EndDate < currentDate.AddDays(1) &&
                    (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())) && x.IsActive,
                _ => x => (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower())) && x.IsActive
            };

        return condition;
    }
    #endregion

    public ApprovalMatrixCountDto GetTrainingRequestCountsForCandidate()
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        return new ApprovalMatrixCountDto
        {
            PendingCount =
                genericRepository.GetCount<TrainingCandidate>(
                    x => x.CandidateId == candidate.Id && !x.IsActionCompleted),
            RejectedCount = genericRepository.GetCount<TrainingCandidate>(x =>
                x.CandidateId == candidate.Id && x.IsActionCompleted && !x.IsApproved),
            ApprovedCount = genericRepository.GetCount<TrainingCandidate>(x =>
                x.CandidateId == candidate.Id && x.IsActionCompleted && x.IsApproved)
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

    public List<GetClientOrganizationUsersDto> GetAllClientCandidatesForTraining(Guid trainingId, int requestAction)
    {
        var clientId = userService.GetUserId;

        var clientUser = genericRepository.GetById<User>(clientId)
                         ?? throw new NotFoundException("The following client has not been registered to our system.");

        var training = genericRepository.GetById<Training>(trainingId)
            ?? throw new NotFoundException("The following training could not be found.");

        var organizationId = clientUser.OrganizationId ?? throw new NotFoundException("The following client user is not a organizational user.");

        var organization = genericRepository.GetById<Organization>(organizationId)
                           ?? throw new NotFoundException("The following organization could not be found.");

        var organizationUsers = genericRepository.Get<User>(x => x.OrganizationId == organization.Id && x.Id != clientUser.Id).ToList();

        var trainingCandidates =
            genericRepository.Get<TrainingCandidate>(x =>
                x.TrainingId == training.Id && organizationUsers.Select(z => z.Id).Contains(x.CandidateId) &&
                x.IsOrganizationRequested).ToList();

        var candidateRequests = requestAction switch
        {
            Constants.RequestAction.Pending => trainingCandidates.Where(x => !x.IsActionCompleted)
                .ToList(),
            Constants.RequestAction.Accepted => trainingCandidates.Where(x => x is { IsActionCompleted: true, IsApproved: true })
                .ToList(),
            Constants.RequestAction.Rejected => trainingCandidates.Where(x => x is { IsActionCompleted: true, IsApproved: false })
                .ToList(),
            Constants.RequestAction.Available => [],
            _ => throw new ArgumentOutOfRangeException(nameof(requestAction), requestAction, null)
        };

        var result = new List<GetClientOrganizationUsersDto>();

        var organizationDetails = new GetOrganizationDto()
        {
            Id = organization.Id,
            Name = organization.Name,
            Address = organization.Address,
            ImageUrl = organization.ImageUrl,
            IsActive = organization.IsActive,
            Description = organization.Description,
        };

        if (requestAction == Constants.RequestAction.Available)
        {
            var rejectedCandidates = trainingCandidates.Where(x =>
                    x is { IsActionCompleted: true, IsApproved: false }).ToList();

            var nonAvailableCandidates = trainingCandidates.Where(x =>
                !rejectedCandidates.Select(z => z.Id).Contains(x.Id)).ToList();

            var availableCandidates =
                organizationUsers.Where(x => !nonAvailableCandidates.Select(z =>
                    z.CandidateId).Contains(x.Id)).ToList();

            result = availableCandidates.Select(x => new GetClientOrganizationUsersDto()
            {
                Id = x.Id,
                Name = x.Name,
                EmailAddress = x.Email ?? "",
                PhoneNumber = x.PhoneNumber ?? "",
                ImageUrl = x.ImageURL,
                Organization = organizationDetails,
                Gender = x.Gender.ToString(),
                DesignationId = x.DesignationId,
                Designation = x.DesignationId != null ? genericRepository.GetById<Designation>(x.DesignationId)?.Title : null,
            }).ToList();
        }
        else
        {
            foreach (var candidateRequest in candidateRequests)
            {
                var candidate = genericRepository.GetById<User>(candidateRequest.CandidateId)
                                ?? throw new NotFoundException("The following candidate has not been registered to our system.");

                result.Add(new GetClientOrganizationUsersDto()
                {
                    TrainingCandidateId = candidateRequest.Id,
                    Id = candidate.Id,
                    Name = candidate.Name,
                    EmailAddress = candidate.Email ?? "",
                    PhoneNumber = candidate.PhoneNumber ?? "",
                    ImageUrl = candidate.ImageURL,
                    RequestedDate = candidateRequest.RequestedDate.ToFormattedDateTime(),
                    ActionDate = candidateRequest.ActionDate?.ToFormattedDateTime(),
                    Remarks = candidateRequest.Remarks,
                    Organization = organizationDetails,
                    Gender = candidate.Gender.ToString(),
                    DesignationId = candidate.DesignationId,
                    Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null,
                });
            }
        }

        return result;
    }
    
    public GetClientOrganizationCandidateCountDto ClientOrganizationNominationsCount(Guid trainingId)
    {
        var clientId = userService.GetUserId;

        var clientUser = genericRepository.GetById<User>(clientId)
                         ?? throw new NotFoundException("The following client has not been registered to our system.");

        var training = genericRepository.GetById<Training>(trainingId)
           ?? throw new NotFoundException("The following training could not be found.");

        var organizationId = clientUser.OrganizationId ?? throw new NotFoundException("The following client user is not a organizational user.");

        var organization = genericRepository.GetById<Organization>(organizationId)
                           ?? throw new NotFoundException("The following organization could not be found.");

        var organizationUsers = genericRepository.Get<User>(x => x.OrganizationId == organization.Id && x.Id != clientUser.Id).ToList();

        var acceptedCandidatesCount = 
            genericRepository.GetCount<TrainingCandidate>(x => 
                x.TrainingId == training.Id && organizationUsers.Select(z => z.Id).Contains(x.CandidateId) && 
                    x.IsOrganizationRequested);

        var nonAcceptedCandidatesCount = organizationUsers.Count - acceptedCandidatesCount;
        
        return new GetClientOrganizationCandidateCountDto
        {
            AcceptedCount = acceptedCandidatesCount,
            AvailableCount = nonAcceptedCandidatesCount
        };
    }
}
