using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Candidate;

namespace CMSTrain.Application.Interfaces.Services;

public interface ICandidateService : ITransientService
{
    GetCandidateDetailsDto GetCandidateDetailsById(Guid candidateId);

    GetCandidateDetailsDto GetCandidateDetailsByAttendanceId(Guid attendanceId);
}