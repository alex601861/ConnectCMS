using CMSTrain.Domain.Entities;
using CMSTrain.Application.Exceptions;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Candidate;
using CMSTrain.Application.DTOs.Organization;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class CandidateService(IGenericRepository genericRepository) : ICandidateService
{
    public GetCandidateDetailsDto GetCandidateDetailsById(Guid candidateId)
    {
        return GetCandidateDetails(candidateId);
    }

    public GetCandidateDetailsDto GetCandidateDetailsByAttendanceId(Guid attendanceId)
    {
        var attendance = genericRepository.GetById<Attendance>(attendanceId)
            ?? throw new NotFoundException("The respective attendance could not be found.");

        return GetCandidateDetails(attendance.CandidateId);
    }

    private GetCandidateDetailsDto GetCandidateDetails(Guid candidateId)
    {
        var candidate = genericRepository.GetById<User>(candidateId)
            ?? throw new NotFoundException("The respective candidate could not be found.");

        return new GetCandidateDetailsDto()
        {
            Id = candidate.Id,
            Name = candidate.Name,
            ImageUrl = candidate.ImageURL,
            EmailAddress = candidate.Email ?? "",
            PhoneNumber = candidate.PhoneNumber ?? "",
            Gender = candidate.Gender.ToString(),
            DesignationId = candidate.DesignationId,
            Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null,
            Organization = candidate.OrganizationId == null ? null : new GetOrganizationDto()
            {
                Id = genericRepository.GetById<Organization>(candidate.OrganizationId)!.Id,
                Name = genericRepository.GetById<Organization>(candidate.OrganizationId)!.Name,
                Description = genericRepository.GetById<Organization>(candidate.OrganizationId)!.Description,
                ImageUrl = genericRepository.GetById<Organization>(candidate.OrganizationId)?.ImageUrl ?? "",
                Address = genericRepository.GetById<Organization>(candidate.OrganizationId)!.Address,
                IsActive = genericRepository.GetById<Organization>(candidate.OrganizationId)!.IsActive,
            }
        };
    }
}