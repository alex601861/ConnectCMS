using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Common.User;
using CMSTrain.Application.DTOs.Candidate;
using CMSTrain.Application.DTOs.Organization;
using CMSTrain.Application.DTOs.Subordinate;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities.Identity;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class SubordinateService(IGenericRepository genericRepository, ICurrentUserService userService)
    : ISubordinateService
{
    public GetSubordinateDto GetSubordinateById(Guid subordinateId)
    {
        var subordinate = genericRepository.GetById<Subordinate>(subordinateId)
                           ?? throw new NotFoundException(
                               "The following subordinate with the specified identifier was not found.");

        var result = new GetSubordinateDto()
        {
            Id = subordinate.Id,
            Name = subordinate.Name,
            Email = subordinate.Email,
            Type = subordinate.SubordinateType.ToString(),
            ContactNumber = subordinate.ContactNumber,
            TrainingCandidateId = subordinate.TrainingCandidateId,
        };

        return result;
    }

    public GetCandidateDetailsDto GetCandidateBySubordinateId(Guid subordinateId)
    {
        var subordinate = genericRepository.GetById<Subordinate>(subordinateId)
                           ?? throw new NotFoundException(
                               "The following subordinate with the specified identifier was not found.");
        
        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x => x.Id == subordinate.TrainingCandidateId && x.IsApproved)
            ?? throw new NotFoundException("The following candidate has not been accepted to the respective training.");
        
        var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId)
            ?? throw new NotFoundException("The following candidate has not been registered to our system.");
        
        var designation = candidate.DesignationId == null ? null : genericRepository.GetById<Designation>(candidate.DesignationId)
            ?? throw new NotFoundException("The following designation could not be found.");
        
        var organization = candidate.OrganizationId != null
            ? genericRepository.GetById<Organization>(candidate.OrganizationId)
            : null;

        return new GetCandidateDetailsDto()
        {
            Id = candidate.Id,
            Name = candidate.Name,
            DesignationId = designation?.Id,
            Designation = designation?.Title,
            PhoneNumber = candidate.PhoneNumber ?? "",
            EmailAddress = candidate.Email ?? "",
            Gender = candidate.Gender.ToString(),
            ImageUrl = candidate.ImageURL,
            Organization = organization == null
                ? null
                : new GetOrganizationDto()
                {
                    Id = organization.Id,
                    Name = organization.Name,
                    ImageUrl = organization.ImageUrl,
                    Address = organization.Address,
                    Description = organization.Description,
                    IsActive = organization.IsActive
                }
        };
    }
    
    public List<GetSubordinateDto> GetSubordinateDetails(Guid trainingId, int pageNumber, int pageSize,
        out int rowCount, string? search = null, int? type = null)
    {
        var candidateId = userService.GetUserId;

        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                                    x.TrainingId == trainingId &&
                                    x.CandidateId == candidateId &&
                                    x.IsActionCompleted && x.IsApproved)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been registered to the respective training.");

        // var subordinates = genericRepository.GetPagedResult<Subordinate>(pageNumber, pageSize, out rowCount, x => x.TrainingCandidateId == trainingCandidate.Id).ToList();

        var subordinates = type switch
        {
            Constants.SubordinateType.Junior => genericRepository.GetPagedResult<Subordinate>(pageNumber, pageSize,
                out rowCount,
                x => x.TrainingCandidateId == trainingCandidate.Id && x.SubordinateType == SubordinateType.Junior &&
                     (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()))).ToList(),

            Constants.SubordinateType.Peer => genericRepository.GetPagedResult<Subordinate>(pageNumber, pageSize,
                out rowCount,
                x => x.TrainingCandidateId == trainingCandidate.Id && x.SubordinateType == SubordinateType.Peer &&
                     (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()))).ToList(),
            Constants.SubordinateType.Supervisor => genericRepository.GetPagedResult<Subordinate>(pageNumber, pageSize,
                out rowCount,
                x => x.TrainingCandidateId == trainingCandidate.Id && x.SubordinateType == SubordinateType.Supervisor &&
                     (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()))).ToList(),
            _ => genericRepository.GetPagedResult<Subordinate>(pageNumber, pageSize, out rowCount,
                x => x.TrainingCandidateId == trainingCandidate.Id &&
                     (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()))).ToList(),
        };

        var result = subordinates.Select(x => new GetSubordinateDto()
        {
            Id = x.Id,
            ContactNumber = x.ContactNumber,
            TrainingCandidateId = x.TrainingCandidateId,
            Name = x.Name,
            Email = x.Email,
            Type = x.SubordinateType.ToString()
        }).ToList();

        return result;
    }

    public List<GetSubordinateDto> GetSubordinateDetails(Guid trainingId)
    {
        var candidateId = userService.GetUserId;

        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                                    x.TrainingId == trainingId &&
                                    x.CandidateId == candidateId &&
                                    x.IsActionCompleted && x.IsApproved)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been registered to the respective training.");

        var subordinates = genericRepository.Get<Subordinate>(x => x.TrainingCandidateId == trainingCandidate.Id)
            .ToList();

        var result = subordinates.Select(x => new GetSubordinateDto()
        {
            Id = x.Id,
            ContactNumber = x.ContactNumber,
            TrainingCandidateId = x.TrainingCandidateId,
            Name = x.Name,
            Email = x.Email,
            Type = x.SubordinateType.ToString()
        }).ToList();

        return result;
    }

    public List<GetSubordinateDto> GetSubordinateDetailsForTrainingCandidate(Guid trainingCandidateId, int pageNumber,
        int pageSize, out int rowCount)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been registered to the respective training.");

        var subordinates = genericRepository.GetPagedResult<Subordinate>(pageNumber, pageSize, out rowCount,
            x => x.TrainingCandidateId == trainingCandidate.Id).ToList();

        var result = subordinates.Select(x => new GetSubordinateDto()
        {
            Id = x.Id,
            ContactNumber = x.ContactNumber,
            TrainingCandidateId = x.TrainingCandidateId,
            Name = x.Name,
            Email = x.Email,
            Type = x.SubordinateType.ToString()
        }).ToList();

        return result;
    }

    public List<GetSubordinateDto> GetSubordinateDetailsForTrainingCandidate(Guid trainingCandidateId)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been registered to the respective training.");

        var subordinates = genericRepository.Get<Subordinate>(x => x.TrainingCandidateId == trainingCandidate.Id)
            .ToList();

        var result = subordinates.Select(x => new GetSubordinateDto()
        {
            Id = x.Id,
            ContactNumber = x.ContactNumber,
            TrainingCandidateId = x.TrainingCandidateId,
            Name = x.Name,
            Email = x.Email,
            Type = x.SubordinateType.ToString()
        }).ToList();

        return result;
    }

    public GetSubordinateDto GetSubordinateDetails(Guid trainingId, SubordinateType subordinateType)
    {
        var candidateId = userService.GetUserId;

        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                                    x.TrainingId == trainingId &&
                                    x.CandidateId == candidateId &&
                                    x.IsActionCompleted && x.IsApproved)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been registered to the respective training.");

        var subordinate = genericRepository.GetFirstOrDefault<Subordinate>(x =>
                              x.TrainingCandidateId == trainingCandidate.Id && x.SubordinateType == subordinateType)
                          ?? throw new NotFoundException(
                              "The following subordinate has not been registered to the respective training.");

        return new GetSubordinateDto()
        {
            Id = subordinate.Id,
            Name = subordinate.Name,
            Email = subordinate.Email,
            Type = subordinate.SubordinateType.ToString(),
            ContactNumber = subordinate.ContactNumber,
            TrainingCandidateId = trainingCandidate.Id
        };
    }

    // TODO: Addition of Email Variations to Lower Cases.
    public void InsertSubordinateForCandidates(CreateSubordinateDto subordinate)
    {
        var candidateId = userService.GetUserId;

        var trainingCandidate = 
            genericRepository.GetFirstOrDefault<TrainingCandidate>(x => 
                x.TrainingId == subordinate.TrainingId && x.CandidateId == candidateId && 
                x.IsActionCompleted && x.IsActionCompleted) 
            ?? throw new NotFoundException("The following candidate has not been registered to the respective training.");

        var subordinateDetails = subordinate.SubordinateDetails;

        var trainingCandidateSubordinates = genericRepository.Get<Subordinate>(x => 
            x.TrainingCandidateId == trainingCandidate.Id).ToList();

        if (trainingCandidateSubordinates.Any(x => 
                x.SubordinateType == subordinateDetails.Type || 
                x.Email == subordinateDetails.Email || 
                x.ContactNumber == subordinateDetails.ContactNumber))
        {
            throw new BadRequestException(
                "Subordinate could not be registered.", 
                ["The following subordinate with the same type or the contact details has already been registered to the respective candidate's training details."]);
        }
        
        var subordinateModel = new Subordinate()
        {
            TrainingCandidateId = trainingCandidate.Id,
            Name = subordinateDetails.Name,
            Email = subordinateDetails.Email.ToLower(),
            ContactNumber = subordinateDetails.ContactNumber,
            SubordinateType = subordinateDetails.Type
        };

        genericRepository.Insert(subordinateModel);
    }

    public void InsertSubordinateForCandidates(CreateClientSubordinateDto subordinate)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(subordinate.TrainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been registered to the respective training.");

        var subordinateDetails = subordinate.SubordinateDetails;

        var trainingCandidateSubordinates = genericRepository.Get<Subordinate>(x => 
            x.TrainingCandidateId == trainingCandidate.Id).ToList();

        if (trainingCandidateSubordinates.Any(x => 
                x.SubordinateType == subordinateDetails.Type || 
                x.Email == subordinateDetails.Email || 
                x.ContactNumber == subordinateDetails.ContactNumber))
        {
            throw new BadRequestException(
                "Subordinate could not be registered.", 
                ["The following subordinate with the same type or the contact details has already been registered to the respective candidate's training details."]);
        }
        
        var subordinateModel = new Subordinate()
        {
            TrainingCandidateId = trainingCandidate.Id,
            Name = subordinateDetails.Name,
            Email = subordinateDetails.Email.ToLower(),
            ContactNumber = subordinateDetails.ContactNumber,
            SubordinateType = subordinateDetails.Type
        };

        genericRepository.Insert(subordinateModel);
    }
}