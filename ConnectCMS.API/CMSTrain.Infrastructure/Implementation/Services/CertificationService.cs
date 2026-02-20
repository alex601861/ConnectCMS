using CMSTrain.Helper;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.Exceptions;
using CMSTrain.Domain.Common.Property;
using CMSTrain.Application.Common.User;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Certification;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class CertificationService(IGenericRepository genericRepository, 
    ICurrentUserService userService, 
    IHangfireService hangfireService) : ICertificationService
{
    public GetCertificationDetails GetCertificationDetailsById(Guid certificationId)
    {
        var certification = genericRepository.GetById<Certificate>(certificationId);

        if (certification == null) return new GetCertificationDetails();

        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(certification.TrainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been assigned to the respective training.");

        var training = genericRepository.GetById<Training>(trainingCandidate.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");
        
        var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId)
                        ?? throw new NotFoundException("The following candidate could not be found.");
        
        var salutation = candidate.Gender switch
        {
            GenderType.Male => "Mr.",
            GenderType.Female => "Ms.",
            _ => null
        };
        
        var certificationDetailsOverview = new CertificationDetails()
        {
            Training = training.Title,
            Candidate = $"{salutation} {candidate.Name}",
            Date = certification.CreatedAt.ToFormattedDateTime(),
        };

        return new GetCertificationDetails()
        {
            Id = certification.Id,
            TrainingId = training.Id,
            TrainingCandidateId = certification.TrainingCandidateId,
            CertificationDetails = certificationDetailsOverview
        };
    }

    public GetCertificationDetails GetCertificationDetailsByTrainingId(Guid trainingId)
    {
        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");

        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");
        
        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x => x.TrainingId == training.Id && x.CandidateId == candidate.Id && x.IsApproved)
                                ?? throw new NotFoundException("The respective candidate has not been approved to the following training.");

        var certification = genericRepository.GetFirstOrDefault<Certificate>(x => 
            x.TrainingCandidateId == trainingCandidate.Id);

        if (certification == null) return new GetCertificationDetails();
        
        var salutation = candidate.Gender switch
        {
            GenderType.Male => "Mr.",
            GenderType.Female => "Ms.",
            _ => null
        };
        
        var certificationDetailsOverview = new CertificationDetails()
        {
            Training = training.Title,
            Candidate = $"{salutation} {candidate.Name}",
            Date = certification.CreatedAt.ToFormattedDateTime(),
        };

        return new GetCertificationDetails()
        {
            Id = certification.Id,
            TrainingId = training.Id,
            TrainingCandidateId = certification.TrainingCandidateId,
            CertificationDetails = certificationDetailsOverview
        };
    }
    
    public GetCertificationDetails GetCertificationDetailsByTrainingCandidateId(Guid trainingCandidateId)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(trainingCandidateId)
                                ?? throw new NotFoundException("The respective candidate has not been approved to the following training.");

        var certification = genericRepository.GetFirstOrDefault<Certificate>(x => 
            x.TrainingCandidateId == trainingCandidate.Id);

        if (certification == null) return new GetCertificationDetails();
        
        var training = genericRepository.GetById<Training>(trainingCandidate.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");
        
        var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId)
                        ?? throw new NotFoundException("The following candidate could not be found.");
        
        var salutation = candidate.Gender switch
        {
            GenderType.Male => "Mr.",
            GenderType.Female => "Ms.",
            _ => null
        };
        
        var certificationDetailsOverview = new CertificationDetails()
        {
            Training = training.Title,
            Candidate = $"{salutation} {candidate.Name}",
            Date = certification.CreatedAt.ToFormattedDateTime(),
        };

        return new GetCertificationDetails()
        {
            Id = certification.Id,
            TrainingId = training.Id,
            TrainingCandidateId = certification.TrainingCandidateId,
            CertificationDetails = certificationDetailsOverview
        };
    }
    
    public void IssueTrainingCandidateCertification(IssueCertificationDto issueCertification)
    {
        var trainingCandidate = genericRepository.GetById<TrainingCandidate>(issueCertification.TrainingCandidateId)
                                ?? throw new NotFoundException(
                                    "The following candidate has not been registered to the respective training.");

        var certification =
            genericRepository.GetFirstOrDefault<Certificate>(x => x.TrainingCandidateId == trainingCandidate.Id);

        if (certification != null) return;
        
        var training = genericRepository.GetById<Training>(trainingCandidate.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId)
                        ?? throw new NotFoundException(
                            "The following candidate has not been registered to our system.");
        
        var classes = genericRepository.Get<Class>(x => x.TrainingId == training.Id).ToList();

        var attendances = genericRepository
            .Get<Attendance>(x => classes.Select(z => z.Id).Contains(x.ClassId) && x.CandidateId == candidate.Id)
            .ToList();
        
        var isAttendancesApproved = attendances.All(x => x is { IsActionCompleted: true, IsApproved: true });
        
        var trainingInspections = genericRepository.Get<TrainingInspection>(x 
                                      => x.TrainingId == trainingCandidate.TrainingId).ToList()
                                ?? throw new NotFoundException("The following training inspection could not be found.");
        
        var questionnaires = genericRepository.Get<Questionnaire>(x => 
            x.TrainingInspectionId != null && trainingInspections.Select(z => z.Id).Contains(x.TrainingInspectionId.Value)).ToList();

        var userResponses = genericRepository.GetCount<UserResponse>(x =>
            questionnaires.Select(z => z.Id).Contains(x.QuestionId) && x.CandidateId == candidate.Id && x.IsAnsweredByCandidate);

        var strategicResponses = genericRepository.GetCount<StrategicTraitResponse>(x =>
            questionnaires.Select(z => z.Id).Contains(x.QuestionnaireId) && x.CandidateId == candidate.Id);

        var isInspectionModuleAnswered = questionnaires.Count == userResponses + strategicResponses;

        if (!isAttendancesApproved || !isInspectionModuleAnswered) return;
        
        var certificateModel = new Certificate()
        {
            TrainingCandidateId = trainingCandidate.Id,
            Score = new KeyValueProperty()
            {
                Key = "Score",
                Value = "N/A"
            },
            Remarks = string.Empty,
            Description = new KeyValueProperty()
            {
                Key = "Description",
                Value = "N/A"
            }
        };

        genericRepository.Insert(certificateModel);
    }

    public void IssueTrainingCertifications(Guid trainingId)
    {
        var training = genericRepository.GetById<Training>(trainingId)
                       ?? throw new NotFoundException("The following training could not be found.");
        
        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x => 
            x.TrainingId == training.Id && x.IsActionCompleted && x.IsApproved).ToList();

        foreach (var trainingCandidate in trainingCandidates)
        {
            var certification =
                genericRepository.GetFirstOrDefault<Certificate>(x => x.TrainingCandidateId == trainingCandidate.Id);

            if (certification != null) continue;
            
            var candidate = genericRepository.GetById<User>(trainingCandidate.CandidateId)
                            ?? throw new NotFoundException(
                                "The following candidate has not been registered to our system.");

            var certificateModel = new Certificate()
            {
                TrainingCandidateId = trainingCandidate.Id,
                Score = new KeyValueProperty
                {
                    Key = "Score",
                    Value = "N/A"
                },
                Remarks = string.Empty,
                Description = new KeyValueProperty()
                {
                    Key = "Description",
                    Value = "N/A"
                },
                CreatedBy = candidate.Id
            };

            genericRepository.Insert(certificateModel);
        }
        
        hangfireService.RemoveRecurringJobs(trainingId.ToString());
    }
}